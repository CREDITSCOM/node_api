mkdir ..\NodeAPIClient\Api
thrift.exe -r --gen csharp -out ../NodeAPIClient/Api ../third-party/thrift-interface-definitions/api.thrift
thrift.exe -r --gen csharp -out ../NodeAPIClient/Api ../third-party/thrift-interface-definitions/apidiag.thrift
thrift.exe -r --gen csharp -out ../NodeAPIClient/Api ../third-party/thrift-interface-definitions/apiexec.thrift
