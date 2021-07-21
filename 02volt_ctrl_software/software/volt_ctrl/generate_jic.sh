#/bin/sh
rm -rf flashconv
mkdir flashconv
chmod 777 ../../prj/output_files/*.sof
cp ../../prj/output_files/*.sof ./flashconv/hwimage.sof
cp *.elf ./flashconv/swimage.elf
cd flashconv
chmod 777 swimage.elf
sof2flash --input=hwimage.sof  --output=hwimage.flash --epcs -verbose
elf2flash --input=swimage.elf --output=swimage.flash --epcs --after=hwimage.flash  --verbose
nios2-elf-objcopy --input-target srec --output-target ihex swimage.flash  swimage.hex
rm -rf ../../../myoutput_files
mkdir ../../../myoutput_files
cp swimage.hex ../../../myoutput_files/swimage.hex
cp hwimage.sof ../../../myoutput_files/hwimage.sof
cp ../generate_jic.cof ../../../generate_jic.cof
cp ../generate_jic.tcl ../../../generate_jic.tcl
cd ../