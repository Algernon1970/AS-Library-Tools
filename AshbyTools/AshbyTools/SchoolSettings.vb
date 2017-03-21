Module SchoolSettings
    Public classCTXString As String = "OU=Subject Groups, OU=Security Groups, OU=AS Groups, OU=Ashby School, DC=as, DC=Internal"
    Public groupsCTXString As String = "OU=AS Groups, OU=Ashby School, DC=as, DC= Internal"
    Public staffGroupsCTXString As String = "OU=Staff Groups,OU=Security Groups,OU=AS Groups, OU=Ashby School, DC=as, DC= Internal"
    Public usersCTXString As String = "OU=AS Users, OU=Ashby School, DC=as, DC=Internal"
    Public tutorsCTXString As String = "OU=Tutor Groups, OU=Security Groups, OU=AS Groups, OU=Ashby School, DC=as, DC=Internal"
    Public yearCTXString As String = "OU=Distribution Groups, OU=AS Groups, OU=Ashby School, DC=as, DC=Internal"
    Public studentOUPATH As String = "OU=20{0} Students, OU=Students, OU=School Users, OU=AS Users, OU=Ashby School, DC=as, DC=internal"
    Public studentADPath As String = "OU=20{0} Students, OU=Students, OU=School Users, {1}"
    Public tlou As String = "dc=as, dc=intenal"
    Public adUser As String = "ASBromcomADSyncer"
    Public adPass As String = "THw8DCzcPMPwqNC5zBUF"

    Public emailServer As String = "svr-exchange1.as.internal"
    Public emailUser As String = "ASBromcomADSyncer@ashbyschool.org.uk"
    Public emailPass As String = "THw8DCzcPMPwqNC5zBUF"
    Public emailFrom As String = "ASBromcomSync noreply@ashbyschool.org.uk"
    Public emailTo As String = "itsupport@ashbyschool.org.uk"
    Public emailSubject As String = "Bromcom Sync Message"

    Public soapUser As String = "petessoaptest"
    Public soapPass As String = "purple"
    Public currentStudentsFilter As String = "GETDATE() BETWEEN StartDate AND ISNULL(EndDate, GETDATE())"
    Public currentStaffFilter As String = "GETDATE() BETWEEN StartDate AND ISNULL(EndDate, GETDATE())"
    Public futureStudents As String = "StartDate > GETDATE()"
    Public leaversFilter As String = "EndDate < GETDATE()"

    Public userHomeDrive As String = "N:"
    Public userPrincipalSuffix As String = "@ashbyschool.org.uk"

    Public fileServerName As String = "svr-file*.as.internal"

    Public domain As String = "as.internal"
    Public emailDomain As String = "@ashbyschool.org.uk"

    Public columns() = {"AdmissionNo", "StudentID", "Forename", "Surname", "ClassName", "Subject", "TutorGroup", "YearGroup", "VleUser"}
End Module
