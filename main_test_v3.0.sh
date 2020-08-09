#!/bin/sh
#updat 20180329 AMD cpu testing;stress will be reboot tesing 
#update 201801017 updae sn flash get
#update V2.0 2019.3.25 get hdd INFO
#update v3.0 自动更新OOB，自动更新raid FW，更新主机SN
#@performance
#@perl-runtime
#@perl-web
#@php



ntpdate 192.168.10.10
main_test="/root/main_test"
if [ -d /root/main_test ]; then
	echo "Test Director ok"
else
	echo "Now unzip the file"
	if [ -f /root/main_test_v3.0.tar.gz ];then
		echo "now unzip test files"
		tar -zxvf main_test_v3.0.tar.gz 
	else
		echo "main_test_v2.0.tar.gz file does not exist"
	 	exit
	fi	
fi
#get MAC active key
cd $main_test/SN_PN
mac_s=`./ipmicfg -m |grep -i mac|awk -F "=" '{print $2}'`
key_s=`$main_test/SN_PN/key/key $mac_s`
		
	
if [ -f /usr/local/bin/ipmitool ];then
	echo "软件已经安装完成，将进行压力测试!!"
	#use ipmicfg get SN
	cd $main_test
	$main_test/ipmicfg/ipmicfg -fru list |grep -i "Product serial number" |awk -F "=" '{print $2}'> ZJsn.log
	serial_number=`cut -c2- ZJsn.log`
        $main_test/ipmicfg/ipmicfg -fru list |grep -i "product name (pn)" |awk -F "=" '{print $2}'> pn.log
	pn_name=`cut -c2- pn.log`
	ZJname="${pn_name}_${serial_number}"
	#ZJname="Test-server8888"
	if [ -f $main_test/reboot.finish ];then
		echo "***____reboot server test finish!____***"
	else
		echo "server reboot test" >>$ZJname.log
		sockets=`dmidecode -t processor |grep -i "SOCKET Designation" |sort|uniq|wc -l`
		if [ $sockets -ge 4 ] ;then
			echo "---now 4cpu reboot loop  20  testing----"
			cd $main_test
			sh reboot4.sh
			cat reboot.log >>$ZJname.log
		elif [ $sockets -eq 2 ]; then
			#statements
			echo "---now 2 cpu reboot 10 loop testing---"
			cd $main_test
			sh reboot.sh
			cat reboot.log >>$ZJname.log
		else
			echo "---now 1 cpu reboot 10 loop testing---"
			cd $main_test
			sh reboot.sh
			cat reboot.log >>$ZJname.log
		fi
	fi
	
	if [ -f $main_test/gethw.flg ];then
	echo "______HWinfo get finished!!_______"
    else
	echo "#######################################################" >>$ZJname.log
	echo "#                                                     #" >>$ZJname.log
	echo "#    Start "$ZJname" get HWINFO                       #" >>$ZJname.log
	echo "#                                                     #" >>$ZJname.log
	echo "#######################################################" >>$ZJname.log

	#get date
	echo "Start time: `date +%Y-%m-%d-%H:%M`">>$ZJname.log
	# get mboard info
	echo "Mainboard info:" >>$ZJname.log
	#dmidecode | grep "Product Name" | awk -F: '{print $2}'|sort|uniq >>$ZJname.log
	echo "Mainboared BIOS version" >>$ZJname.log
	dmidecode -t bios |grep -i "version:">>$ZJname.log
	echo "Server Name" >>$ZJname.log
	#dmidecode | grep "Product Name" | awk -F: '{print $2}'|sort|uniq >>$ZJname.log
	echo "---------------------------------------------------">>$ZJname.log
	# get cpu model and number
	echo "---------------------------------------------------" >>$ZJname.log
	echo "CPU information:" >>$ZJname.log
	dmidecode -t processor |grep -i -E "Socket Designation:|Family:|Version:|id:|Signature:|Core Count:|Thread Count:" >>$ZJname.log
	echo "----------------------------------------------------" >>$ZJname.log
	#get mem total and dimms
	echo "Mem information:" >>$ZJname.log
	dmidecode -t memory |egrep -i "size|part number|Configured Clock Speed" |egrep -i -v "NO DIMM|NO MODULE|unknow" >>$ZJname.log
	#get HDD info for 3108
	/opt/MegaRAID/storcli/storcli64 /c0 show >>$ZJname.log
	#get HDD info FOR 3008 
	$main_test/sas3ircu 0 display |egrep -i "Firmware Revision|Model number" >>$ZJname.log
	echo "----------get-BMC MAC Adress:----------" >>$ZJname.log
	$main_test/ipmicfg/ipmicfg -m|grep MAC >>$ZJname.log
        echo "------get IPMI fru ps/pn/pat-----------------" >>$ZJname.log
	$main_test/ipmicfg/ipmicfg -fru list |egrep -i "Ps|pn|pat">>$ZJname.log
	echo "-----------------------------------">>$ZJname.log
	echo "Intel Ethernet info:" >>$ZJname.log
	lspci -v | grep Ethernet | awk -F: '{print $3}' >>$ZJname.log
	ifconfig -a | egrep "eth| inet addr"  >>$ZJname.log
	echo "--------NIC MAC--------">>$ZJname.log
	ifconfig -a | grep -i "ether"  >>$ZJname.log
	echo "--------NIC MAC--------">>$ZJname.log
	echo "--------get intel OPA INFO-----">>$ZJname.log
	lspci|grep -i omni >>$ZJname.log
	echo "--------get FC Adapter info:" >> $ZJname.log
	lspci | grep -i emulex
	result1=$?
	if [ $result1 == 0 ];then
	echo "Emulex FC Adapter info:" >>$ZJname.log
	hbacmd listhbas >>$ZJname.log
	else
	echo "Qlogic FC Adapter info:" >>$ZJname.log
	ls -1c /sys/class/scsi_host/host*/optrom_fw_version | cut -d'/' -f 5 |  xargs -I {} sh -c 'grep -Hv "zz" /sys/class/scsi_host/{}/optrom*version'>>$ZJname.log
	qaucli -pr fc -i all | grep -i -E "HBA ID|HBA Port|Port Name|BIOS Version|Running Firmware Version|FCode|EFI">>$ZJname.log
	fi
	echo "-----------------------------------">>$ZJname.log
	echo "-------------IPMI FRU PN/PS/PAT-----------------">>$ZJname.log
	$main_test/ipmicfg/ipmicfg -fru list |egrep -i "Ps|pn|pat">>$ZJname.log
	echo "-------------OS SMBIOS SN/TAG-------------------">>$ZJname.log
	dmidecode|egrep -i "asset tag: n1|serial number: B|product name:">>$ZJname.log
	#get nvdia DEVICE
	echo "----------------------Get nvidia GPU info-----------------------------------" >>$ZJname.log	
	echo "----------------------------------------------------------------------------" >>$ZJname.log
	lspci |grep -i nvidia >>$ZJname.log
	#ipmi sensor
	echo "--------------get ipmi sensor----------------------------------">>$ZJname.log
	modprobe ipmi_devintf
	ipmitool sdr >>$ZJname.log
	sleep 5	
	#nvme device
	echo "----------------get NVME disk---------------------------------" >>$ZJname.log
	ls /dev/nvme* >>$ZJname.log
	date +%Y-%m-%d-%H:%M>>$ZJname.log
	echo "-----get infiniband card MAC----------">>$ZJname.log
	mlxfwmanager | grep -i port >>$ZJname.log
	echo "-----Get server Hard HDD -----"  >>$ZJname.log
	echo "----------------------------------------------------------------------------" >>$ZJname.log
	echo "---------------Server HWinfo get Pass-------------------------------------- ">>$ZJname.log
	echo "----------------------------------------------------------------------------" >>$ZJname.log
	touch gethw.flg
	fi
	
	echo "Stress Server cpu and memory HDD" 
	echo "*********************************************************#" >>$ZJname.log
	echo "#                                                        #" >>$ZJname.log
	echo "#    Start "$ZJname" Stress Testing                      #" >>$ZJname.log
	echo "#                                                        #" >>$ZJname.log
	echo "**********************************************************" >>$ZJname.log
	echo "CPU and MEM stress HDD begin" >>$ZJname.log
	date +%Y-%m-%d-%H:%M>>$ZJname.log
	cpus=`grep -i "model name" /proc/cpuinfo |wc -l`
	IPADDR=`/sbin/ifconfig -a|grep inet|grep -v 127.0.0.1|grep -v inet6|awk '{print $2}'|tr -d "addr:"`
	sockets=`dmidecode -t processor |grep -i "SOCKET Designation" |sort|uniq|wc -l`
	c_num=`grep -i "model name" /proc/cpuinfo |wc -l`
	#c_m=$(($c_num/))
	if [ $sockets -ge 4 ] ;then
		   echo "------------Now 4 CPU stress loop 16 Hours-------"
		   echo "---stress----- host----"
			num1=`cat /root/main_test/stress.flg`
			if [ $num1 -eq 13 ];then
				echo "server stress test $num1 loop in 16 complete!!!"
			else
				echo "****Thes loop $num1 test,start `date +%Y-%m-%d-%H:%M:%S`****"
		   		echo "****Thes loop $num1 test,start `date +%Y-%m-%d-%H:%M:%S`****">>$ZJname.log
				ipmitool sdr >>$ZJname.sensor.log&
				stressapptest -s 1800 -C ${c_num}  --pause_delay 10000000 -l $ZJname.stress.log >/dev/null 2>&1
				let num_t=$num1+1
				echo $num_t >/root/main_test/stress.flg
				echo "****Thes loop $num1 test,end `date +%Y-%m-%d-%H:%M:%S`****"
				echo "****Thes loop $num1 test,end `date +%Y-%m-%d-%H:%M:%S`****">>$ZJname.log
				reboot
			fi
	elif [ $sockets -eq 2 ] ;then

		    echo "------Now 2CPU stress test 6 Hour"
		    date +%Y-%m-%d-%H:%M>>$ZJname.log
			num1=`cat /root/main_test/stress.flg`
			if [ $num1 -eq 13 ];then
				echo "server stress test $num1 loop in 16 complete!!!"
			else
				echo "****Thes loop $num1 test,start `date +%Y-%m-%d-%H:%M:%S`****"
		   		echo "****Thes loop $num1 test,start `date +%Y-%m-%d-%H:%M:%S`****">>$ZJname.log
				 ipmitool sdr >>$ZJname.sensor.log&
				stressapptest -s 1800 -C ${c_num}  --pause_delay 10000000 -l $ZJname.stress.log >/dev/null 2>&1
				let num_t=$num1+1
				echo $num_t >/root/main_test/stress.flg
				echo "****Thes loop $num1 test,end `date +%Y-%m-%d-%H:%M:%S`****"
				echo "****Thes loop $num1 test,end `date +%Y-%m-%d-%H:%M:%S`****">>$ZJname.log
				reboot
			fi
	else
			echo "------Now 1CPU stress test 6 Hour"
		    date +%Y-%m-%d-%H:%M>>$ZJname.log
			num1=`cat /root/main_test/stress.flg`
			if [ $num1 -eq 13 ];then
				echo "server stress test $num1 loop in 16 complete!!!"
			else
				echo "****Thes loop $num1 test,start `date +%Y-%m-%d-%H:%M:%S`****"
		   		echo "****Thes loop $num1 test,start `date +%Y-%m-%d-%H:%M:%S`****">>$ZJname.log
				 ipmitool sdr >>$ZJname.sensor.log&
				stressapptest -s 1800 -C ${c_num} --pause_delay 10000000 -l $ZJname.stress.log >/dev/null 2>&1
				let num_t=$num1+1
				echo $num_t >/root/main_test/stress.flg
				echo "****Thes loop $num1 test,end `date +%Y-%m-%d-%H:%M:%S`****"
				echo "****Thes loop $num1 test,end `date +%Y-%m-%d-%H:%M:%S`****">>$ZJname.log
				reboot
			fi
	fi
	
	cat stress.log |grep -v "Log:" >>$ZJname.log	
	date +%Y-%m-%d-%H:%M>>$ZJname.log
	echo "CPU and memory HDD stress Pass">>$ZJname.log
	ipmitool sdr |egrep -i "CPU|FAN"|grep -v "no reading"
	sleep 5
	ipmitool sdr |egrep -i "cpu|fan"|grep -v "no reading" >>$ZJname.log
	echo "*****Now we're going to test it for 8k write/rewrite read/reread random write/read***"
	date +%Y-%m-%d-%H:%M>>$ZJname.log
	if [ -f $main_test/iozone.flg ];then 
	    echo "***iozone test finish***"
	else
	    sh $main_test/fdisk.sh
	    touch $main_test/iozone.flg
	fi
	date +%Y-%m-%d-%H:%M>>$ZJname.log
	#up load file
	#directory mount
	mkdir /root/testlog
	nfs_file=`mount -t nfs 192.168.10.10:/mnt/log_dirs /root/testlog;echo $?`
	if [ ! $nfs_file = 0 ];then echo "*****Log director not create*****";exit;fi
	cd /root/testlog
	if [ -d `date +%Y-%m-%d` ]; then
		echo "exist `date +%Y-%m-%d` directory"
	else
		mkdir `date +%Y-%m-%d`
	fi
	echo "Server test log upload" >>$ZJname.log
	cd /root/testlog/`date +%Y-%m-%d`
#	cp $main_test/$ZJname.log /root/testlog/`date +%Y-%m-%d`
	if [ -f /root/testlog/`date +%Y-%m-%d`/$ZJname.log ]; then
		echo "------------!!! exist $ZJname.log file upload finish!!!------------"
		cp $main_test/$ZJname.log /root/testlog/`date +%Y-%m-%d`/$ZJname.log-A
		cp $main_test/$ZJname.sensor.log /root/testlog/`date +%Y-%m-%d`/$ZJname.sensor.log-A
		cp $main_test/$ZJname.stress.log /root/testlog/`date +%Y-%m-%d`/$ZJname.stress.log-A
	else
		echo "*************************************"
		echo " No exist $ZJname.log file,upload log "
		echo "*************************************"
		cp $main_test/$ZJname*.log /root/testlog/`date +%Y-%m-%d`
		#echo "------------The will sleep 8 hours,please verify--------------"
		#echo "log" >st.flg
		#exit
	fi
	#clear all Disk information
	sleep 15
	for clear_disk in `cat /root/main_test/all_disk`;
	do
		echo "clear ${clear_disk} information">>$ZJname.log
		dd if=/dev/zero of=/dev/${clear_disk} bs=512 count=1
	done
	echo "--------------------------------------">>$ZJname.log
	echo "**************************************">>$ZJname.log
	echo "*************All Test Pass************">>$ZJname.log
	echo "********Please Showdown system********">>$ZJname.log
	echo "--------------------------------------">>$ZJname.log
	echo -e "\033[32m \033[01m ##################################### \033[0m"
	echo -e "\033[32m \033[01m ###### Server test Completed ######## \033[0m"
	echo -e "\033[32m \033[01m ##################################### \033[0m"
	echo -e "\033[32m \033[01m ##################################### \033[0m"
	
else
	echo "install software to localhost"
	rpm -ivh QConvergeConsoleCLI-2.3.00-11.x86_64.rpm
	#install stress
	cd $main_test
	\cp -r -f pci.ids  /usr/share/hwdata/
	cd $main_test/stressapptest-1.0.6_autoconf
	./configure && make && make install
	cd $main_test
	#rpm -ivh kmod*
	wget ftp://192.168.10.10/MLNX_OFED_LINUX-4.3-1.0.1.0-rhel7.4-x86_64.tgz .
	yum list
	yum -y install gtk2 atk cairo tcl tk
	tar zxvf MLNX_OFED_LINUX-4.3-1.0.1.0-rhel7.4-x86_64.tgz
	cd MLNX_OFED_LINUX-4.3-1.0.1.0-rhel7.4-x86_64
	echo y |./mlnxofedinstall
	cd $main_test
	rpm -ivh storcli-1.14.12-1.noarch.rpm
	cd $main_test/ipmitool-1.8.18/ &&./configure && make && make install
	sleep 10
	cd $main_test/iozone3_471/src/current &&  make linux-AMD64
	echo "------------Emulex firmware update--------------"
	lspci | grep -i emulex
	result=$?
	if [ $result == 0 ];then
		cd /root/
		wget ftp://192.168.10.10/emulex-firmware-update.tar.gz .
		tar xvzf emulex-firmware-update.tar.gz
		cd /root/emulex-firmware-update
		tar xvzf hbacmd.tar.gz
		cd hbacmd/
		./install.sh -q -s=0 -m=3
		cd /root/emulex-firmware-update
		mv lancer_A12.2.299.27.grp /root
		sh fcup.sh | tee -a $ZJname.log
	else
		echo "----------Emulex card not found-----------"
	fi		
	reboot
fi
