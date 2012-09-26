require 'fileutils'

# Set the file attributes to normal. Investigate how to do this the ruby way
if (System::IO::File.Exists(fileInfo.FullDestinationPath))
	System::IO::File.SetAttributes(fileInfo.FullDestinationPath, System::IO::FileAttributes.Normal)
end

# Copy the file to its destination
FileUtils.cp(fileInfo.FullSourcePath, fileInfo.FullDestinationPath)
