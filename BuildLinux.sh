#!/bin/bash
#
# Based on Script by user 'Fyzxs' from the RunUO forums (http://www.runuo.com/community/threads/my-installation-experience-with-setup-script.518599/)
#
 
# Location of the server files
UO_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
 
echo
# Checking if Mono needs to be installed
HAS_MONO=$(mono -V | grep "Mono JIT compiler version")
if [ ${#HAS_MONO} -eq 0 ]
then
    echo "Mono not found... Aborting"
    exit
else
    echo "Mono detected. Continuing"
fi
 
echo
# Compiling ThreesandUO
echo "Compiling ThreesandUO"
COMPILE_CMD="dmcs -out:${UO_PATH}/ThreesandUO.exe -d:MONO -optimize+ -unsafe -r:System,System.Configuration.Install,System.Data,System.Drawing,System.EnterpriseServices,System.Management,System.Security,System.ServiceProcess,System.Web,System.Web.Services,System.Windows.Forms,System.Xml -nowarn:219 -recurse:${UO_PATH}/Server/*.cs"
echo "${COMPILE_CMD}"
${COMPILE_CMD}
echo "Compilation of ThreesandUO is complete"
 
echo
# Creating the ThreesandUO.exe.config
echo "Creating ThreesandUO.exe.config"
LIBZ_TARGET=$(locate libz | grep -o --max-count=1 libz.*)
echo "<configuration>" > ${UO_PATH}/ThreesandUO.exe.config
echo " <dllmap dll=\"libz\" target=\"${LIBZ_TARGET}\"/>" >> ${UO_PATH}/ThreesandUO.exe.config
echo "</configuration>" >> ${UO_PATH}/ThreesandUO.exe.config

echo "ThreesandUO set up is complete"
echo "Place init script in /etc/init.d to enable init commands"