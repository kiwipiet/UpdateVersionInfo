# UpdateVersionInfo

A command line utility that assists you in updating all your application version information for your Windows, Android and iOS applications.

    UpdateVersionInfo.exe:
    
    help                                         : Show the following help text
    
    version|v (Default)
        /v /major                                : A numeric major version number greater than zero. (Int32) (Default = 1) (More or equal to 0)
        /m /minor                                : A numeric minor number greater than zero. (Int32) (Default = 0) (More or equal to 0)
        /b /build                                : A numeric build number greater than zero. (Int32) (Default = 0) (More or equal to 0)
        /r /revision                             : A numeric revision number greater than zero. (Int32) (Default = 0) (More or equal to 0)
        /p /path /versioncspath                  : The path to a C# file to update with version information. (String)
        /a /androidmanifest /androidmanifestpath : The path to an android manifest file to update with version information. (String)
        /t /touchplist /touchplistpath           : The path to an iOS plist file to update with version information. (String)


##TODO

  * Update AssemblyInformationalVersion

Copied idea from: https://github.com/soltechinc/soltechxf/tree/master/UpdateVersionInfo
