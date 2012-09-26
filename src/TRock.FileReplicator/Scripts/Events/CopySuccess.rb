log.Invoke("[" + fileInfo.Fileset.Name + "] " + fileInfo.FileName + " >> " + fileInfo.Fileset.DestinationPath)

if (fileInfo.Retries > 0 && fileInfo.Data.LockingProcesses != nil)
    fileInfo.Data.LockingProcesses.each { |process| puts process }
end