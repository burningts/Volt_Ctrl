Range = 30;

VoutArr = [0.72,0.16,5.84,7.12
           3.68,1.54,6.32,12.05];

%Unit:V
Vout1 = 0.72; 
Vout2 = 0.16; 
Vout3 = 5.84;
Vout4 = 7.12;

for i = 1:17
    for j = 1:4
        DAC_data1 = (VoutArr(i,j) / Range) * 4096;
        DAC_data_hex1 = dec2hex(round(DAC_data1));

        DAC_data2 = (Vout(i,j)) / Range) * 4096;
        DAC_data_hex2 = dec2hex(round(DAC_data2));

        DAC_data3 = (Vout3 / Range) * 4096;
        DAC_data_hex3 = dec2hex(round(DAC_data3));

        DAC_data4 = (Vout4 / Range) * 4096;
        DAC_data_hex4 = dec2hex(round(DAC_data4));
    end

        if(length(DAC_data_hex1) == 1)
            DAC_data_hex1_all = ['0',DAC_data_hex1];
            fprintf(' WriteByteData[0] = 0x10;\n');
            fprintf(' WriteByteData[1] = 0x%s;\n',DAC_data_hex1_all);
        elseif(length(DAC_data_hex1) == 2)
            fprintf(' WriteByteData[0] = 0x10;\n');
            fprintf(' WriteByteData[1] = 0x%s;\n',DAC_data_hex1);
        else
            DAC_data_hex1_all = ['1',DAC_data_hex1];
            DAC_data_hex1_all_H = DAC_data_hex1_all(1:2);
            DAC_data_hex1_all_L = DAC_data_hex1_all(3:4);
            fprintf(' WriteByteData[0] = 0x%s;\n',DAC_data_hex1_all_H);
            fprintf(' WriteByteData[1] = 0x%s;\n',DAC_data_hex1_all_L);
        end

        if(length(DAC_data_hex2) == 1)
            DAC_data_hex2_all = ['0',DAC_data_hex2];
            fprintf(' WriteByteData[2] = 0x20;\n');
            fprintf(' WriteByteData[3] = 0x%s;\n',DAC_data_hex2_all);
        elseif(length(DAC_data_hex2) == 2)
            fprintf(' WriteByteData[2] = 0x20;\n');
            fprintf(' WriteByteData[3] = 0x%s;\n',DAC_data_hex2);
        else
            DAC_data_hex2_all = ['2',DAC_data_hex2];
            DAC_data_hex2_all_H = DAC_data_hex2_all(1:2);
            DAC_data_hex2_all_L = DAC_data_hex2_all(3:4);
            fprintf(' WriteByteData[2] = 0x%s;\n',DAC_data_hex2_all_H);
            fprintf(' WriteByteData[3] = 0x%s;\n',DAC_data_hex2_all_L);
        end

        if(length(DAC_data_hex3) == 1)
            DAC_data_hex3_all = ['0',DAC_data_hex3];
            fprintf(' WriteByteData[4] = 0x30;\n');
            fprintf(' WriteByteData[5] = 0x%s;\n',DAC_data_hex3_all);
        elseif(length(DAC_data_hex3) == 2)
            fprintf(' WriteByteData[4] = 0x30;\n');
            fprintf(' WriteByteData[5] = 0x%s;\n',DAC_data_hex3);
        else
            DAC_data_hex3_all = ['3',DAC_data_hex3];
            DAC_data_hex3_all_H = DAC_data_hex3_all(1:2);
            DAC_data_hex3_all_L = DAC_data_hex3_all(3:4);
            fprintf(' WriteByteData[4] = 0x%s;\n',DAC_data_hex3_all_H);
            fprintf(' WriteByteData[5] = 0x%s;\n',DAC_data_hex3_all_L);
        end

        if(length(DAC_data_hex4) == 1)
            DAC_data_hex4_all = ['0',DAC_data_hex4];
            fprintf(' WriteByteData[6] = 0x40;\n');
            fprintf(' WriteByteData[7] = 0x%s;\n',DAC_data_hex4_all);
        elseif(length(DAC_data_hex4) == 2)
            fprintf(' WriteByteData[6] = 0x40;\n');
            fprintf(' WriteByteData[7] = 0x%s;\n',DAC_data_hex4);
        else
            DAC_data_hex4_all = ['4',DAC_data_hex4];
            DAC_data_hex4_all_H = DAC_data_hex4_all(1:2);
            DAC_data_hex4_all_L = DAC_data_hex4_all(3:4);
            fprintf(' WriteByteData[6] = 0x%s;\n',DAC_data_hex4_all_H);
            fprintf(' WriteByteData[7] = 0x%s;\n',DAC_data_hex4_all_L);
        end

end


