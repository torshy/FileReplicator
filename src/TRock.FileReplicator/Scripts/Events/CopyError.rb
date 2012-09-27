log.Invoke("Unable to copy " + fileInfo.FileName + ". " + exception.Message)

if (fileset.KillLockingProcess)
    log.Invoke("Killing locking processes!")
    lockingProcesses = fileInfo.KillLockingProcesses()
    
    #Here we can schedule a relaunch of the locking process, or display a messagebox prompting the user to relaunch it
    lockingProcesses.each { |process| puts process + " is locking " + fileInfo.FullDestinationPath }    
    fileInfo.Data.LockingProcesses = lockingProcesses
end
    
#If there's a need to retry the copy event, invoke the retryCopy delegate
if(fileInfo.Retries < 5)
    log.Invoke("Adding to retry queue")
    retryCopy.Invoke()
end