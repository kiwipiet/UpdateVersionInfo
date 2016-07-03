# UpdateVersionInfo

    UpdateVersionInfo.exe:
    
    help                                         : Show the following help text
    
    version|v (Default)
        /a /androidmanifest /androidmanifestpath : The path to an android manifest file to update with version information. (String)
        /b /build                                : A numeric build number greater than zero. (Int32) (Default = 0) (More or equal to 0)
        /m /minor                                : A numeric minor number greater than zero. (Int32) (Default = 0) (More or equal to 0)
        /p /path /versioncspath                  : The path to a C# file to update with version information. (String)
        /r /revision                             : A numeric revision number greater than zero. (Int32) (Default = 0) (More or equal to 0)
        /t /touchplist /touchplistpath           : The path to an iOS plist file to update with version information. (String)
        /v /major                                : A numeric major version number greater than zero. (Int32) (Default = 1) (More or equal to 0)
        
Copied idea from: https://github.com/soltechinc/soltechxf/tree/master/UpdateVersionInfo
