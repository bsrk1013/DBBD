#pragma once
#include <string>
#include "Buffer.h"
#include "Cell.h"

namespace DBBD {
	class Deserialize
	{
		// Value Read
	public:
		template<typename T>
		static void read(Buffer& buffer, T& value) {
			char* dataBuffer = buffer.readByteBlock(sizeof(T));
			memcpy(&value, dataBuffer, sizeof(T));
		}
		static void read(Buffer& buffer, float& value);
		static void read(Buffer& buffer, std::string& value);
		static void read(Buffer& buffer, std::wstring& value);
		static void read(Buffer& buffer, char* value);
		static void read(Buffer& buffer, Cell* value);

		// Array Read
	public:
		template<typename T, size_t size>
		static void readArray(Buffer& buffer, T(&values)[size]) {
			unsigned int arraySize;
			read(buffer, arraySize);
			for (unsigned int i = 0; i < arraySize; i++) {
				T value;
				read(buffer, value);
				values[i] = value;
			}
		}
		template<typename T1, typename T2>
		static void readVector(Buffer& buffer, T1& vec) {
			unsigned int vecSize;
			read(buffer, vecSize);
			for (unsigned int i = 0; i < vecSize; i++) {
				T2 value;
				read(buffer, value);
				vec.push_back(value);
			}
		}
	};
}