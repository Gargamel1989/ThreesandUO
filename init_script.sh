#!/bin/bash
# /etc/init.d/threesanduo_server
 
### BEGIN INIT INFO
# Provides:   runuo
# Required-Start: $local_fs $remote_fs
# Required-Stop:  $local_fs $remote_fs
# Should-Start:   $network
# Should-Stop:    $network
# Default-Start:  2 3 4 5
# Default-Stop:   0 1 6
# Short-Description:    RunUO Server
# Description:    Init script for RunUO server
### END INIT INFO
 
# Edit these variables to match your system
 
# User that should run the server
USERNAME="uo-admin"

# The base directory of the server files
SERVER_DIR=
 
# Path to runuo server directory
UOPATH="$SERVER_DIR/ThreesandUO"
 
# Executable that starts RunUo
SERVICE="ThreesandUO.exe"

# The path that contains the UO client files
THREESANDUO_CLIENT_FILES_DIR=

# The hostname of this server
THREESANDUO_HOSTNAME=

# Log file location
LOG_FILE="$SERVER_DIR/log/ThreesandUO.log"
 
# STOP -- Do Not Edit Below Here
# =============================
 
INVOCATION="nohup mono ${UOPATH}/${SERVICE} > $LOG_FILE 2>&1 &"
 
ME=`whoami`
as_user() {
        if [ $ME == $USERNAME ] ; then
                env THREESANDUO_CLIENT_FILES_DIR=$THREESANDUO_CLIENT_FILES_DIR THREESANDUO_HOSTNAME=$THREESANDUO_HOSTNAME bash -c "$1"
        else
                env THREESANDUO_CLIENT_FILES_DIR=$THREESANDUO_CLIENT_FILES_DIR THREESANDUO_HOSTNAME=$THREESANDUO_HOSTNAME su $USERNAME -s /bin/bash -c "$1"
        fi
}
 
is_running(){
        # Checks for the RunUO.exe
        # returns true if it exists.
        if ps ax | grep -v grep | grep "$SERVICE" > /dev/null
        then
                return 0
        fi
        return 1
}
 
uo_start() {
        cd $UOPATH
        as_user "$INVOCATION"
        #
        # Waiting for the server to start
        #
        seconds=0
        until ps ax | grep -v grep | grep -v -i SCREEN | grep "$SERVICE" > /dev/null
        do
                sleep 1
                seconds=$seconds+1
                if [[ $seconds -eq 5 ]]
                then
                        echo "Still not running, waiting a while longer..."
                fi
                if [[ $seconds -ge 120 ]]
                then
                        echo "Failed to start, aborting."
                        exit 1
                fi
        done
        sleep 3
        echo "$SERVICE is running."
}
 
uo_stop(){
        seconds=0
        PID=$(ps ax | grep -v grep | grep $SERVICE | awk '{print $1}')
        while  ps ax | grep -v grep | grep "$SERVICE" > /dev/null
        do
                as_user "kill $PID"
                sleep 1
                seconds=$seconds+1
                if [[ $seconds -eq 5 ]]
                then
                        echo "Still not shut down, waiting a while longer..."
                fi
                if [[ $seconds -ge 120 ]]
                then
                        echo "Failed to shut down, forcing kill."
                        as_user "kill -9 $PID"
                        seconds=0
                fi
        done
        sleep 3
        echo  "$SERVICE is now shut down"
}
 
case "$1" in
        start)
                # Starts the server
                if is_running; then
                        echo "Server already running."
                else
                        uo_start
                fi
                ;;
        stop)
                # Stops the server
                if is_running; then
                        uo_stop
                else
                        echo "No running server."
                fi
                ;;
        restart)
                # Restarts the server
                if is_running; then
                        uo_stop
                else
                        echo "No running server, starting it..."
                fi
                sleep 3
                uo_start
                ;;
        status)
                # Shows server status
                if is_running
                then
                        echo "$SERVICE is running."
                else
                        echo "$SERVICE is not running."
                fi
                ;;
        help|--help|-h)
                echo "Usage: $0 COMMAND"
                echo
                echo "Available commands:"
                echo -e "   start \t\t Starts the server"
                echo -e "   stop \t\t Stops the server"
                echo -e "   restart \t\t Restarts the server"
                ;;
        *)
                echo "No such command, see $0 help"
                exit 1
                ;;
 
esac
 
exit 0
