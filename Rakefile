require 'albacore'

task :default => :build do
end

desc "Assembly info generator"
assemblyinfo :assemblyinfo do |asm|
  asm.version = "0.10.0.0"
  asm.file_version = "0.10.0.0"
  asm.company_name = "TRock"
  asm.product_name = "File Replicator"
  asm.title = "File Replicator"
  asm.description = "Simple file replicator"
  asm.copyright = "Copyright 2012"  
  asm.custom_attributes :AssemblyInformationalVersionAttribute => "0.10.0.0 RC1"
  asm.output_file = "src/CommonAssemblyInfo.cs"
end

desc "Build solution"
msbuild :build => :assemblyinfo do |msb|
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Clean, :Build ]
  msb.solution = "TRock.FileReplicator.sln"
end

zip :zip => :build do | zip |
    zip.directories_to_zip "src/TRock.FileReplicator/bin/Release"
    zip.output_file = "filereplicator.zip"
    zip.output_path = File.dirname(__FILE__)
end

desc "Generate setup"
exec :setup => :build do |cmd|
	cmd.command = getInnoPath + "/iscc.exe"
	cmd.parameters File.dirname(__FILE__) + "/installer.iss"
end

module InnoSetup
  extend Rake::DSL if defined?(Rake::DSL)

  EXECUTABLE = "iscc.exe"

  def self.present?
    candidate = ENV['PATH'].split(File::PATH_SEPARATOR).detect do |path|
      File.exist?(File.join(path, EXECUTABLE)) &&
        File.executable?(File.join(path, EXECUTABLE))
    end

    return true if candidate
  end
end

def getInnoPath
  unless InnoSetup.present?
    # if not found, add InnoSetup to the PATH
    path = File.join(ENV['ProgramFiles'], 'Inno Setup 5')
    path.gsub!(File::SEPARATOR, File::ALT_SEPARATOR)
    ENV['PATH'] = "#{ENV['PATH']}#{File::PATH_SEPARATOR}#{path}"	
  end
  fail "You need InnoSetup installed" unless InnoSetup.present?
  path
end