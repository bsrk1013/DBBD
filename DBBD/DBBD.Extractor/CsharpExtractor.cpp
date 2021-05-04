#include "CsharpExtractor.h"
#include <string>

using namespace std;

CsharpExtractor::CsharpExtractor(std::filesystem::path basePath, const vector<string>& fileList)
	: BaseExtractor(ExtractorType::Csharp, basePath, fileList) {

}

void CsharpExtractor::writeHeader(std::ofstream& ofs)
{
	ofs << "using DBBD;" << endl;
	ofs << "using System;" << endl;
	ofs << "using System.Linq;" << endl;
	ofs << "using System.Text;" << endl;
	ofs << "using System.Collections.Generic;" << endl;
	ofs << endl;
}

void CsharpExtractor::writeCell(std::ofstream& ofs)
{
	writeContentsHeader(ofs);
	writeCellContents(ofs);
	ofs << "}" << endl;
}

void CsharpExtractor::writeProtocol(std::ofstream& ofs)
{
	writeContentsHeader(ofs);

	for (auto info : headerInfoList) {
		if (info.fileType == XmlElementType::Protocol) {
			writeProtocolContents(ofs, info.base);
			break;
		}
	}

	ofs << "}" << endl;
}

void CsharpExtractor::writeConst(std::ofstream& ofs, std::string fileName, bool isFirst)
{
	auto firstInfo = headerInfoList.front();
	if (firstInfo.fileType == XmlElementType::Comment)
		ofs << "// " << firstInfo.value << endl;
	ofs << "static class " << fileName << endl;
	ofs << "{" << endl;

	ofs << "\tpublic enum Value" << endl;
	ofs << "\t{" << endl;
	for (auto info : headerInfoList) {
		if (info.fileType == XmlElementType::Comment)
			continue;
		ofs << "\t\t" << info.name << " = " << info.value << "," << endl;
	}
	ofs << "\t}" << endl << endl;

	ofs << "\tprivate static Dictionary<Value, string> stringMap = new Dictionary<Value, string>" << endl;
	ofs << "\t{" << endl;
	for (auto info : headerInfoList) {
		if (info.fileType == XmlElementType::Comment)
			continue;

		ofs << "\t\t{ Value." << info.name << ", \"" << info.name << "\" }," << endl;
	}
	ofs << "\t};" << endl << endl;

	ofs << "\tpublic static string Get(Value value)" << endl;
	ofs << "\t{" << endl;
	ofs << "\t\tif (!stringMap.ContainsKey(value))" << endl;
	ofs << "\t\t{" << endl;
	ofs << "\t\t\treturn \"\";" << endl;
	ofs << "\t\t}" << endl;
	ofs << "\t\treturn stringMap[value];" << endl;
	ofs << "\t}" << endl;

	ofs << "}" << endl << endl;
}

void CsharpExtractor::writeContentsHeader(std::ofstream& ofs)
{
	vector<FileInfo> realContents;
	for (auto contentsInfo : contentsInfoList) {
		if (contentsInfo.fileType != XmlElementType::Property) { continue; }
		realContents.push_back(contentsInfo);
	}

	for (auto info : headerInfoList) {
		switch (info.fileType) {
		case XmlElementType::Comment: {
			ofs << "// " << info.value << endl;
			break;
		}
		case XmlElementType::Protocol:
		case XmlElementType::Cell: {
			if (info.fileType == XmlElementType::Cell) {
				ofs << "class " << info.name << " : DBBD.I" << info.base << endl;
			}
			else if (info.fileType == XmlElementType::Protocol) {
				ofs << "class " << info.name << " : DBBD." << info.base << endl;
			}
			ofs << "{" << endl;
			ofs << "\tpublic " << info.name << "()" << endl;
			ofs << "\t{" << endl;
			if (info.fileType == XmlElementType::Cell) {
				if (realContents.size() > 0) {
					ofs << "\t\tfingerPrinter.AddRange(Enumerable.Repeat(false, " << realContents.size() << "));" << endl;
				}
			}
			else if (info.fileType == XmlElementType::Protocol) {
				size_t pos = info.type.find(".");
				if (pos >= 256)
					throw std::exception("illegal protocol type format");
				auto namespaceName = info.type.substr(0, pos);
				auto valueName = info.type.substr(pos + 1, info.type.size() - 1);

				ofs << "\t\ttypeId = (uint)" << namespaceName << ".Value." << valueName << ";" << endl;
				if (realContents.size() > 0) {
					ofs << "\t\tfingerPrinter.AddRange(Enumerable.Repeat(false, " << realContents.size() << "));" << endl;
				}
			}
			ofs << "\t}" << endl << endl;
			break;
		}
		}
	}
}

void CsharpExtractor::writeCellContents(std::ofstream& ofs)
{
	vector<FileInfo> realContents;
	for (auto contentsInfo : contentsInfoList) {
		if (contentsInfo.fileType != XmlElementType::Property) { continue; }
		realContents.push_back(contentsInfo);
	}
	vector<FileInfo> header;
	for (auto headerInfo : headerInfoList) {
		if (headerInfo.fileType == XmlElementType::Cell) { header.push_back(headerInfo); break; }
	}

	ofs << "\tpublic override void Serialize(DBBD.Buffer buffer)" << endl;
	ofs << "\t{" << endl;
	if (realContents.size() > 0) {
		ofs << "\t\tDBBD.Serizlie.Write(buffer, fingerPrinter);" << endl;
		for (size_t i = 0; i < realContents.size(); i++) {
			auto info = realContents[i];
			ofs << "\t\tif (fingerPrinter[" << i << "]) { " << "DBBD.Serialize.Write(buffer, " << info.name << "); }" << endl;
		}
	}
	ofs << "\t}" << endl << endl;

	ofs << "\tpublic override void Deserialize(DBBD.Buffer buffer)" << endl;
	ofs << "\t{" << endl;
	if (realContents.size() > 0) {
		ofs << "\t\tDBBD.Deserialize.Read(buffer, out fingerPrinter);" << endl;
		for (size_t i = 0; i < realContents.size(); i++) {
			auto info = realContents[i];
			ofs << "\t\tif (fingerPrinter[" << i << "]) { " << "DBBD.Deserialize.Read(buffer, out " << info.name << "); }" << endl;
		}
	}
	ofs << "\t}" << endl << endl;

	ofs << "\tpublic override uint GetLength()" << endl;
	ofs << "\t{" << endl;
	ofs << "\t\tuint totalLength = 0;" << endl;
	if (realContents.size() > 0) {
		ofs << "\t\ttotalLength += (uint)(sizeof(uint) + fingerPrinter.Count);" << endl;
		for (size_t i = 0; i < realContents.size(); i++) {
			auto info = realContents[i];
			ofs << "\t\tif (fingerPrinter[" << i << "]) { totalLength += (uint)(" << getLength(info.base, info.type, info.name) << "); }" << endl;
		}
	}
	ofs << "\t\treturn totalLength;" << endl;
	ofs << "\t}" << endl << endl;

	if (realContents.size() <= 0) { return; }

	for (size_t i = 0; i < realContents.size(); i++) {
		auto info = realContents[i];

		string newName = "";
		for (size_t i = 0; i < info.name.size(); i++) {
			char c = info.name[i];
			if (i == 0) {
				newName += toupper(c);
			}
			else {
				newName += c;
			}
		}

		ofs << "\tpublic " << getPropertyType(info.base, info.type) << " " << newName << " { get { return " << info.name << "; } set { " << info.name << " = value; fingerPrinter[" << i << "] = true; } }" << endl;
	}

	ofs << endl;
	ofs << "\tprivate List<bool> fingerPrinter = new List<bool>();" << endl;

	for (size_t i = 0; i < realContents.size(); i++) {
		auto info = realContents[i];

		string newName = "";
		for (size_t i = 0; i < info.name.size(); i++) {
			char c = info.name[i];
			if (i == 0) {
				newName += toupper(c);
			}
			else {
				newName += c;
			}
		}

		ofs << "\tprivate " << getPropertyType(info.base, info.type) << " " << info.name << ";" << endl;
	}
}

void CsharpExtractor::writeProtocolContents(std::ofstream& ofs, std::string base)
{
	vector<FileInfo> realContents;
	for (auto contentsInfo : contentsInfoList) {
		if (contentsInfo.fileType != XmlElementType::Property) { continue; }
		realContents.push_back(contentsInfo);
	}

	ofs << "\tpublic override void Serialize(DBBD.Buffer buffer)" << endl;
	ofs << "\t{" << endl;
	ofs << "\t\tbase.WriteHeader(buffer, GetLength());" << endl;
	if (realContents.size() > 0) {
		ofs << "\t\tDBBD.Serialize.Write(buffer, fingerPrinter);" << endl;
		for (size_t i = 0; i < realContents.size(); i++) {
			auto info = realContents[i];
			ofs << "\t\tif (fingerPrinter[" << i << "]) { " << "DBBD.Serialize.Write(buffer, " << info.name << "); }" << endl;
		}
	}
	ofs << "\t}" << endl << endl;

	ofs << "\tpublic override void Deserialize(DBBD.Buffer buffer)" << endl;
	ofs << "\t{" << endl;
	ofs << "\t\tbase.ReadHeader(buffer);" << endl;
	if (realContents.size() > 0) {
		ofs << "\t\tDBBD.Deserialize.Read(buffer, out fingerPrinter);" << endl;
		for (size_t i = 0; i < realContents.size(); i++) {
			auto info = realContents[i];
			ofs << "\t\tif (fingerPrinter[" << i << "]) { " << "DBBD.Deserialize.Read(buffer, out " << info.name << "); }" << endl;
		}
	}
	ofs << "\t}" << endl << endl;

	ofs << "\tpublic override uint GetLength()" << endl;
	ofs << "\t{" << endl;
	ofs << "\t\tuint totalLength = 0;" << endl;
	ofs << "\t\ttotalLength += (uint)(base.GetLength());" << endl;
	if (realContents.size() > 0) {
		ofs << "\t\ttotalLength += (uint)(sizeof(uint) + fingerPrinter.Count);" << endl;
		for (size_t i = 0; i < realContents.size(); i++) {
			auto info = realContents[i];
			ofs << "\t\tif (fingerPrinter[" << i << "]) { totalLength += (uint)(" << getLength(info.base, info.type, info.name) << "); }" << endl;
		}
	}
	ofs << "\t\treturn totalLength;" << endl;
	ofs << "\t}" << endl;

	if (realContents.size() <= 0) { return; }

	ofs << endl;

	for (size_t i = 0; i < realContents.size(); i++) {
		auto info = realContents[i];

		string newName = "";
		for (size_t i = 0; i < info.name.size(); i++) {
			char c = info.name[i];
			if (i == 0) {
				newName += toupper(c);
			}
			else {
				newName += c;
			}
		}

		ofs << "\tpublic " << getPropertyType(info.base, info.type) << " " << newName << " { get { return " << info.name << "; } set { " << info.name << " = value; fingerPrinter[" << i << "] = true; } }" << endl;
	}

	ofs << endl;
	ofs << "\tprivate List<bool> fingerPrinter = new List<bool>();" << endl;

	for (size_t i = 0; i < contentsInfoList.size(); i++) {
		auto info = contentsInfoList[i];

		switch (info.fileType) {
		case XmlElementType::Comment:
			if (i == contentsInfoList.size() - 1)
				break;

			ofs << "\t// " << info.value << endl;
			break;
		case XmlElementType::Property:
			string newName = "";
			for (size_t i = 0; i < info.name.size(); i++) {
				char c = info.name[i];
				if (i == 0) {
					newName += toupper(c);
				}
				else {
					newName += c;
				}
			}

			ofs << "\tprivate " << getPropertyType(info.base, info.type) << " " << info.name << ";" << endl;
			break;
		}
	}
}

string CsharpExtractor::getLength(string base, string type, string name) {
	switch (HashCode(type.c_str())) {
	case HashCode("int64"):
	case HashCode("uint64"):
	case HashCode("int32"):
	case HashCode("uint32"):
	case HashCode("int16"):
	case HashCode("uint16"):
	case HashCode("double"):
	case HashCode("float"):
	case HashCode("bool"):
	case HashCode("char"):
	case HashCode("byte"):
	case HashCode("sbyte"):
		return "sizeof(" + getPropertyType(base, type) + ")";
		break;
	case HashCode("string"):
		return "sizeof(" + getPropertyType(base, "uint32") + ") + Encoding.UTF8.GetByteCount(" + name + ")";
	default:
		return name + ".GetLength()";
	}
}