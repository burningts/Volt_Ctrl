#!/bin/sh
#
# This file was automatically generated.
#
# It can be overwritten by nios2-flash-programmer-generate or nios2-flash-programmer-gui.
#

#
# Converting SOF File: E:\master_WHU\voltage_control\02volt_ctrl_software\software\volt_ctrl\volt_ctrl.sof to: "..\flash/volt_ctrl_epcs_flash.flash"
#
sof2flash --input="E:/master_WHU/voltage_control/02volt_ctrl_software/software/volt_ctrl/volt_ctrl.sof" --output="../flash/volt_ctrl_epcs_flash.flash" --epcs 

#
# Programming File: "..\flash/volt_ctrl_epcs_flash.flash" To Device: epcs_flash
#
nios2-flash-programmer "../flash/volt_ctrl_epcs_flash.flash" --base=0x11000 --epcs --sidp=0x12070 --id=0x0 --accept-bad-sysid --device=1 --instance=0 '--cable=USB-BlasterII on localhost [USB-1]' --program --erase-all 

#
# Converting ELF File: E:\master_WHU\voltage_control\02volt_ctrl_software\software\volt_ctrl\volt_ctrl.elf to: "..\flash/volt_ctrl_epcs_flash_1_.flash"
#
elf2flash --input="E:/master_WHU/voltage_control/02volt_ctrl_software/software/volt_ctrl/volt_ctrl.elf" --output="../flash/volt_ctrl_epcs_flash_1_.flash" --epcs --after="../flash/volt_ctrl_epcs_flash.flash" 

#
# Programming File: "..\flash/volt_ctrl_epcs_flash_1_.flash" To Device: epcs_flash
#
nios2-flash-programmer "../flash/volt_ctrl_epcs_flash_1_.flash" --base=0x11000 --epcs --sidp=0x12070 --id=0x0 --accept-bad-sysid --device=1 --instance=0 '--cable=USB-BlasterII on localhost [USB-1]' --program --go 

