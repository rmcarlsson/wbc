
setlocal

@rem enter this directory
cd /d %~dp0

set TOOLS_PATH=..\packages\Grpc.Tools.0.15.0\tools\windows_x86
%TOOLS_PATH%\protoc.exe -Iproto -I..\packages\Google.Protobuf.Tools.3.0.0-beta3\tools --csharp_out .  proto\grainbrain.proto 
%TOOLS_PATH%\protoc.exe -Iproto -I..\packages\Google.Protobuf.Tools.3.0.0-beta3\tools --csharp_out .  proto\grainbrain.proto --grpc_out . --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

endlocal