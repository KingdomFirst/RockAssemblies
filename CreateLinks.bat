del .\KFSRockAssemblies
del .\RockAttendedCheckin
del .\RockWeb\Plugins\rocks_kfs
del .\RockWeb\Plugins\cc_newspring 
del .\RockWeb\Content\KFSRockAssets
mklink /J KFSRockAssemblies c:\KFSRepo\Rock\KFSRockAssemblies
mklink /J RockAttendedCheckin c:\KFSRepo\Rock\RockAttendedCheckin 
mklink /J RockWeb\Plugins\rocks_kfs c:\KFSRepo\Rock\KFSRockBlocks
mklink /J RockWeb\Plugins\cc_newspring c:\KFSRepo\Rock\RockAttendedCheckin\cc_newspring
mklink /J RockWeb\Content\KFSRockAssets c:\KFSRepo\Rock\KFSRockAssets
copy /Y KFSRockAssemblies\CLAUDE.md CLAUDE.md