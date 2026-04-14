del .\KFSRockAssemblies
del .\RockAttendedCheckin
del .\RockWeb\Plugins\rocks_kfs
del .\RockWeb\Plugins\cc_newspring 
del .\RockWeb\Content\KFSRockAssets
del .\com.protectmyministry.RockPlugin
del .\RockWeb\Plugins\com.protectmyministry
mklink /J KFSRockAssemblies c:\KFSRepo\Rock\KFSRockAssemblies
mklink /J RockAttendedCheckin c:\KFSRepo\Rock\RockAttendedCheckin 
mklink /J RockWeb\Plugins\rocks_kfs c:\KFSRepo\Rock\KFSRockBlocks
mklink /J RockWeb\Plugins\cc_newspring c:\KFSRepo\Rock\RockAttendedCheckin\cc_newspring
mklink /J RockWeb\Content\KFSRockAssets c:\KFSRepo\Rock\KFSRockAssets
mklink /J com.protectmyministry.RockPlugin C:\KFSRepo\PM-Integrations\rock\com.protectmyministry.RockPlugin_for_RockV16\com.protectmyministry.RockPlugin
mklink /J RockWeb\Plugins\com.protectmyministry C:\KFSRepo\PM-Integrations\rock\com.protectmyministry.RockPlugin_for_RockV16\RockWeb\Plugins\com.protectmyministry
