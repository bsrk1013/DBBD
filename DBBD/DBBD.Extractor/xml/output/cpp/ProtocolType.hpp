#pragma once
#include "DBBD/Cell.h"
#include "DBBD/Request.h"
#include "DBBD/Response.h"
#include "DBBD/Common.hpp"
#include "DBBD/json.hpp"
#include "ProtocolType.hpp"

#include <map>
#include <string>

//Base Protocol
namespace Base {
	static enum Value {
		ServerConnectReq = 1001,
		ServerConnectResp = 1002,
		PingCheckReq = 1003,
		PingCheckResp = 1004,
	};
	static std::map<Value, std::wstring> stringMap = {
		{ Value::ServerConnectReq, L"ServerConnectReq" },
		{ Value::ServerConnectResp, L"ServerConnectResp" },
		{ Value::PingCheckReq, L"PingCheckReq" },
		{ Value::PingCheckResp, L"PingCheckResp" },
	};
	static std::wstring Get(Value value) {
		auto iter = Base::stringMap.find(value);
		if (iter == Base::stringMap.end()) {
			return L"";
		}
		return iter->second;
	}
}
// Relay Protocol
namespace Relay {
	static enum Value {
		RelayNoti = 2001,
	};
	static std::map<Value, std::wstring> stringMap = {
		{ Value::RelayNoti, L"RelayNoti" },
	};
	static std::wstring Get(Value value) {
		auto iter = Relay::stringMap.find(value);
		if (iter == Relay::stringMap.end()) {
			return L"";
		}
		return iter->second;
	}
}
// MailType 
namespace MailType {
	static enum Value {
		None = 0,
	};
	static std::map<Value, std::wstring> stringMap = {
		{ Value::None, L"None" },
	};
	static std::wstring Get(Value value) {
		auto iter = MailType::stringMap.find(value);
		if (iter == MailType::stringMap.end()) {
			return L"";
		}
		return iter->second;
	}
}
