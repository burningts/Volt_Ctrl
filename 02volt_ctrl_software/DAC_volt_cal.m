clc
Range = 30;
k = 0;

LoadInArr = [
            0.32
            0.16
            5.84
            7.12
            3.68
            1.54
            6.32
            12.05
                    ];

%Unit:V
VoutArr = reshape(LoadInArr,4,2);
VoutArr = VoutArr';

% VoutArr = [0.32,0.16,5.84,7.12
%            3.68,1.54,6.32,12.05];

fprintf(' --------------------------------DAC_volt_cal_copy-----------------------------------------\n');
 for i = 1:2
    for j = 1:4
        DAC_data1 = (VoutArr(i,j) / Range) * 4096;
        DAC_data_hex1 = dec2hex(round(DAC_data1));

        if(length(DAC_data_hex1) == 1)
            DAC_data_hex1_all = ['0',DAC_data_hex1];
            fprintf(' WriteByteData[%s] = 0x%s0;\n',num2str(k),num2str(j));
            fprintf(' WriteByteData[%s] = 0x%s;\n',num2str(k+1),DAC_data_hex1_all);
        elseif(length(DAC_data_hex1) == 2)
            fprintf(' WriteByteData[%s] = 0x%s0;\n',num2str(k),num2str(j));
            fprintf(' WriteByteData[%s] = 0x%s;\n',num2str(k+1),DAC_data_hex1);
        else
            DAC_data_hex1_all = [num2str(j),DAC_data_hex1];
            DAC_data_hex1_all_H = DAC_data_hex1_all(1:2);
            DAC_data_hex1_all_L = DAC_data_hex1_all(3:4);
            fprintf(' WriteByteData[%s] = 0x%s;\n',num2str(k),DAC_data_hex1_all_H);
            fprintf(' WriteByteData[%s] = 0x%s;\n',num2str(k+1),DAC_data_hex1_all_L);
        end
        k = k + 2;
    end
 end





