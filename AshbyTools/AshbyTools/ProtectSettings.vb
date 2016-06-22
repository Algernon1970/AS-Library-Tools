Imports System.Configuration
Module ProtectSettings

    Private Sub protectSettings()
        Dim config As Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        config.ConnectionStrings.SectionInformation.ProtectSection(Nothing)
        ' We must save the changes to the configuration file.
        config.Save(ConfigurationSaveMode.Full, True)
    End Sub
End Module
