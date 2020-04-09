#!/bin/sh
add-apt-repository ppa:ubuntu-toolchain-r/test
apt update
apt install libstdc++6
wget http://ftp.de.debian.org/debian/pool/main/t/thrift/thrift-compiler_0.13.0-2_amd64.deb
dpkg -i thrift-compiler_0.13.0-2_amd64.deb
thrift --version
mkdir ../NodeAPIClient/Api
thrift -r --gen csharp -out ../NodeAPIClient/Api ../third-party/thrift-interface-definitions/api.thrift
thrift -r --gen csharp -out ../NodeAPIClient/Api ../third-party/thrift-interface-definitions/apidiag.thrift
thrift -r --gen csharp -out ../NodeAPIClient/Api ../third-party/thrift-interface-definitions/apiexec.thrift
