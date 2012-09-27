log.Invoke("[" + fileset.Name + "] " + fileInfo.FileName + " >> " + fileset.DestinationPath)

if (fileInfo.Retries > 0 && fileInfo.Data.LockingProcesses != nil)
    fileInfo.Data.LockingProcesses.each { |process| puts process }
end