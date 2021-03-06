
#include <unistd.h>
#include <stdio.h>
#include "alt_types.h"
#include "sys/alt_stdio.h"
#include "io.h"
#include "system.h"
#include "sys/alt_cache.h"
#include "altera_avalon_spi.h"
#include "altera_avalon_spi_regs.h"
#include "sys/alt_irq.h"
#include "altera_avalon_pio_regs.h" //pio
#include "altera_avalon_uart_regs.h"
#include "stddef.h"
#include "priv/alt_legacy_irq.h"

#define WR_AD5504_CTRL_ALL_POWERUP_H 0x70
#define WR_AD5504_CTRL_ALL_POWERUP_L 0X3C
#define WR_AD5504_CTRL_ALL_POWERDOWN_H 0x70
#define WR_AD5504_CTRL_ALL_POWERDOWN_L 0X00

#define OE_MSK_1 0X01FFFFFE
#define OE_MSK_2 0X01FFFFFD
#define OE_MSK_3 0X01FFFFFB
#define OE_MSK_4 0X01FFFFF7
#define OE_MSK_5 0X01FFFFEF
#define OE_MSK_6 0X01FFFFDF
#define OE_MSK_7 0X01FFFFBF
#define OE_MSK_8 0X01FFFF7F
#define OE_MSK_9 0X01FFFEFF
#define OE_MSK_10 0X01FFFDFF
#define OE_MSK_11 0X01FFFBFF
#define OE_MSK_12 0X01FFF7FF
#define OE_MSK_13 0X01FFEFFF
#define OE_MSK_14 0X01FFDFFF
#define OE_MSK_15 0X01FFBFFF
#define OE_MSK_16 0X01FF7FFF
#define OE_MSK_17 0X01FEFFFF
#define OE_MSK_18 0X01FDFFFF
#define OE_MSK_19 0X01FBFFFF
#define OE_MSK_20 0X01F7FFFF
#define OE_MSK_21 0X01EFFFFF
#define OE_MSK_22 0X01DFFFFF
#define OE_MSK_23 0X01BFFFFF
#define OE_MSK_24 0X017FFFFF
#define OE_MSK_25 0X00FFFFFF
#define OE_ALL_ENABLE 0X00000000

alt_u8 UartReceiveCnt = 0;
alt_u8 txdata = 0;
alt_u8 rxdata[200];
int IsSettingInfo = 1;
alt_u8 Datalength = 0;

int return_code;
int ret;
int oe_cnt = 0;
int oe_cnt1 = 0;
int oe_cnt2 = 0;
int out_delay = 0;

alt_u8 spi_command_string_tx[200];
alt_u32 oe_msk[25] = {OE_MSK_1, OE_MSK_2, OE_MSK_3, OE_MSK_4, OE_MSK_5,
                      OE_MSK_6, OE_MSK_7, OE_MSK_8, OE_MSK_9, OE_MSK_10,
                      OE_MSK_11, OE_MSK_12, OE_MSK_13, OE_MSK_14, OE_MSK_15,
                      OE_MSK_16, OE_MSK_17, OE_MSK_18, OE_MSK_19, OE_MSK_20,
                      OE_MSK_21, OE_MSK_22, OE_MSK_23, OE_MSK_24, OE_MSK_25};

alt_u8 spi_command_string_rx[10] = "1HELLOABC*";

void IRQ_UART_Interrupts();
void IRQ_init();
void WriteFileData();

//This is the ISR that runs when the SPI Slave receives data

static void spi_rx_isr(void *isr_context)
{

    // alt_printf("ISR :) %x \n", IORD_ALTERA_AVALON_SPI_RXDATA(SPI_BASE));

    //This resets the IRQ flag. Otherwise the IRQ will continuously run.
    IOWR_ALTERA_AVALON_SPI_STATUS(SPI_BASE, 0x0);
}

int main()
{
    //alt_printf("Hello from Nios II!\n");

    // DAC register
    // WR_AD5504_CTRL_SET_ALLDAC_H 0X5 + 12bit data
    // WR_AD5504_CTRL_SET_DAC_A_H 0X1 + 12bit data
    // WR_AD5504_CTRL_SET_DAC_B_H 0X2 + 12bit data
    // WR_AD5504_CTRL_SET_DAC_C_H 0X3 + 12bit data
    // WR_AD5504_CTRL_SET_DAC_D_H 0X4 + 12bit data

    // DAC init
    spi_command_string_tx[0] = WR_AD5504_CTRL_ALL_POWERUP_H;
    spi_command_string_tx[1] = WR_AD5504_CTRL_ALL_POWERUP_L;
    // spi_command_string_tx[0] = WR_AD5504_CTRL_ALL_POWERDOWN_H;
    // spi_command_string_tx[1] = WR_AD5504_CTRL_ALL_POWERDOWN_L;

    //DAC all set 0
    spi_command_string_tx[2] = 0x50;
    spi_command_string_tx[3] = 0x00;

    spi_command_string_tx[4] = 0x00;
    spi_command_string_tx[5] = 0x00;

    IRQ_init();
    // //This registers the Slave IRQ with NIOS
    // ret = alt_ic_isr_register(SPI_IRQ_INTERRUPT_CONTROLLER_ID, SPI_IRQ, spi_rx_isr, (void *)spi_command_string_tx, 0x0);
    // alt_printf("IRQ register return %x \n", ret);

    // //You need to enable the IRQ in the IP core control register as well.
    // IOWR_ALTERA_AVALON_SPI_CONTROL(SPI_BASE, ALTERA_AVALON_SPI_CONTROL_SSO_MSK | ALTERA_AVALON_SPI_CONTROL_IRRDY_MSK);

    // //Just calling the ISR to see if the function is OK.
    // spi_rx_isr(NULL);

    // //DAC power-up
    for (int i = 18; i < 149; i = i + 8)
    {
        IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 1);
        IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[(i - 6 * oe_cnt) / 2 - 1]);
        oe_cnt++;

        return_code = alt_avalon_spi_command(SPI_BASE, 0,
                                             2, spi_command_string_tx,
                                             0, spi_command_string_rx,
                                             0);
    }
    IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, OE_ALL_ENABLE);
    IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 0);
    IOWR_ALTERA_AVALON_PIO_DATA(PIO_LED_BASE, 1);

    //alt_printf("DAC Power up \n");
    while (IsSettingInfo)
        ;

    while (1)
    {

        while (UartReceiveCnt < Datalength)
            ;
        IsSettingInfo = 1;
        // alt_printf("datalength:  %x \n", Datalength);
        // for (int i = 0; i < UartReceiveCnt; i++)
        // {
        //     alt_printf("uart receive:  %x \n", rxdata[i]);
        // }
        WriteFileData();
        //alt_printf("spi_command_string_tx[28] = %x \n", spi_command_string_tx[28]);
        UartReceiveCnt = 0;

        while (1)
        {
            if (UartReceiveCnt >= Datalength)
            {
                break;
            }
            for (int j = 18; j < 149; j = j + 8)
            {
                IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 1);
                IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[(j - 6 * oe_cnt1) / 2 - 1]);
                return_code = alt_avalon_spi_command(SPI_BASE, 0,
                                                     2, &spi_command_string_tx[j],
                                                     0, &spi_command_string_rx[1],
                                                     0);
                return_code = alt_avalon_spi_command(SPI_BASE, 0,
                                                     2, &spi_command_string_tx[j + 2],
                                                     0, &spi_command_string_rx[1],
                                                     0);
                return_code = alt_avalon_spi_command(SPI_BASE, 0,
                                                     2, &spi_command_string_tx[j + 4],
                                                     0, &spi_command_string_rx[1],
                                                     0);
                return_code = alt_avalon_spi_command(SPI_BASE, 0,
                                                     2, &spi_command_string_tx[j + 6],
                                                     0, &spi_command_string_rx[1],
                                                     0);
                oe_cnt1++;
            }
            usleep(10);
            IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, OE_ALL_ENABLE);
            IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 0);
            oe_cnt1 = 0;
            usleep(374);

            for (int k = 18; k < 149; k = k + 8)
            {
                IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 1);
                IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[(k - 6 * oe_cnt2) / 2 - 1]);
                return_code = alt_avalon_spi_command(SPI_BASE, 0,
                                                     2, &spi_command_string_tx[2],
                                                     0, &spi_command_string_rx[1],
                                                     0);
                oe_cnt2++;
            }
            usleep(10);
            IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, OE_ALL_ENABLE);
            IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 0);
            oe_cnt2 = 0;
            usleep(79);
        }
    }

    //DATA TO DAC

    // while (1)
    // {
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 1);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[13]);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, spi_command_string_tx,
    //                                          0, spi_command_string_rx,
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[68],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[70],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[72],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[74],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[15]);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, spi_command_string_tx,
    //                                          0, spi_command_string_rx,
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[68],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[70],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[72],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[74],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[14]);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, spi_command_string_tx,
    //                                          0, spi_command_string_rx,
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[68],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[70],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[72],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[74],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, OE_ALL_ENABLE);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 0);
    //     usleep(300);

    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 1);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[13]);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, spi_command_string_tx,
    //                                          0, spi_command_string_rx,
    //                                          0);

    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[2],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[15]);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, spi_command_string_tx,
    //                                          0, spi_command_string_rx,
    //                                          0);

    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[2],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, oe_msk[14]);
    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, spi_command_string_tx,
    //                                          0, spi_command_string_rx,
    //                                          0);

    //     return_code = alt_avalon_spi_command(SPI_BASE, 0,
    //                                          2, &spi_command_string_tx[2],
    //                                          0, &spi_command_string_rx[3],
    //                                          0);

    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_OE_BASE, OE_ALL_ENABLE);
    //     IOWR_ALTERA_AVALON_PIO_DATA(PIO_LDAC_N_BASE, 0);
    //     usleep(300);
    // }

    //RX is done via interrupts.

    return 0;
}

void IRQ_init()
{

    IOWR_ALTERA_AVALON_UART_STATUS(UART_BASE, 0);

    IOWR_ALTERA_AVALON_UART_CONTROL(UART_BASE, 0X80);
    IOWR_ALTERA_AVALON_UART_DIVISOR(UART_BASE, 0x0364);
    // ??ISR
    alt_ic_isr_register(
        UART_IRQ_INTERRUPT_CONTROLLER_ID,
        UART_IRQ,
        IRQ_UART_Interrupts,
        0x0,
        0x0);
}

void IRQ_UART_Interrupts()
{
    if (!IsSettingInfo)
    {
        rxdata[UartReceiveCnt] = IORD_ALTERA_AVALON_UART_RXDATA(UART_BASE);
        UartReceiveCnt++;
    }
    if (IsSettingInfo)
    {
        Datalength = IORD_ALTERA_AVALON_UART_RXDATA(UART_BASE);
        IsSettingInfo = 0;
    }

    //alt_printf("uart receive:  %c \n", rxdata);
    // txdata = rxdata;
    // while (!(IORD_ALTERA_AVALON_UART_STATUS(UART_BASE) &
    //          ALTERA_AVALON_UART_STATUS_TRDY_MSK))
    //     ;
    // IOWR_ALTERA_AVALON_UART_TXDATA(UART_BASE, txdata);
}

void WriteFileData()
{
    //DAC10
    spi_command_string_tx[18] = rxdata[0];
    spi_command_string_tx[19] = rxdata[1];
    spi_command_string_tx[20] = rxdata[2];
    spi_command_string_tx[21] = rxdata[3];
    spi_command_string_tx[22] = rxdata[4];
    spi_command_string_tx[23] = rxdata[5];
    spi_command_string_tx[24] = rxdata[6];
    spi_command_string_tx[25] = rxdata[7];

    spi_command_string_tx[26] = rxdata[8];
    spi_command_string_tx[27] = rxdata[9];
    spi_command_string_tx[28] = rxdata[10];
    spi_command_string_tx[29] = rxdata[11];
    spi_command_string_tx[30] = rxdata[12];
    spi_command_string_tx[31] = rxdata[13];
    spi_command_string_tx[32] = rxdata[14];
    spi_command_string_tx[33] = rxdata[15];

    spi_command_string_tx[34] = rxdata[16];
    spi_command_string_tx[35] = rxdata[17];
    spi_command_string_tx[36] = rxdata[18];
    spi_command_string_tx[37] = rxdata[19];
    spi_command_string_tx[38] = rxdata[20];
    spi_command_string_tx[39] = rxdata[21];
    spi_command_string_tx[40] = rxdata[22];
    spi_command_string_tx[41] = rxdata[23];
    spi_command_string_tx[42] = rxdata[24];
    spi_command_string_tx[43] = rxdata[25];
    spi_command_string_tx[44] = rxdata[26];
    spi_command_string_tx[45] = rxdata[27];
    spi_command_string_tx[46] = rxdata[28];
    spi_command_string_tx[47] = rxdata[29];
    spi_command_string_tx[48] = rxdata[30];
    spi_command_string_tx[49] = rxdata[31];
    spi_command_string_tx[50] = rxdata[32];
    spi_command_string_tx[51] = rxdata[33];
    spi_command_string_tx[52] = rxdata[34];
    spi_command_string_tx[53] = rxdata[35];
    spi_command_string_tx[54] = rxdata[36];
    spi_command_string_tx[55] = rxdata[37];
    spi_command_string_tx[56] = rxdata[38];
    spi_command_string_tx[57] = rxdata[39];
    spi_command_string_tx[58] = rxdata[40];
    spi_command_string_tx[59] = rxdata[41];
    spi_command_string_tx[60] = rxdata[42];
    spi_command_string_tx[61] = rxdata[43];
    spi_command_string_tx[62] = rxdata[44];
    spi_command_string_tx[63] = rxdata[45];
    spi_command_string_tx[64] = rxdata[46];
    spi_command_string_tx[65] = rxdata[47];
    spi_command_string_tx[66] = rxdata[48];
    spi_command_string_tx[67] = rxdata[49];
    spi_command_string_tx[68] = rxdata[50];
    spi_command_string_tx[69] = rxdata[51];
    spi_command_string_tx[70] = rxdata[52];
    spi_command_string_tx[71] = rxdata[53];
    spi_command_string_tx[72] = rxdata[54];
    spi_command_string_tx[73] = rxdata[55];
    spi_command_string_tx[74] = rxdata[56];
    spi_command_string_tx[75] = rxdata[57];
    spi_command_string_tx[76] = rxdata[58];
    spi_command_string_tx[77] = rxdata[59];
    spi_command_string_tx[78] = rxdata[60];
    spi_command_string_tx[79] = rxdata[61];
    spi_command_string_tx[80] = rxdata[62];
    spi_command_string_tx[81] = rxdata[63];
    spi_command_string_tx[82] = rxdata[64];
    spi_command_string_tx[83] = rxdata[65];
    spi_command_string_tx[84] = rxdata[66];
    spi_command_string_tx[85] = rxdata[67];
    spi_command_string_tx[86] = rxdata[68];
    spi_command_string_tx[87] = rxdata[69];
    spi_command_string_tx[88] = rxdata[70];
    spi_command_string_tx[89] = rxdata[71];
    spi_command_string_tx[90] = rxdata[72];
    spi_command_string_tx[91] = rxdata[73];
    spi_command_string_tx[92] = rxdata[74];
    spi_command_string_tx[93] = rxdata[75];
    spi_command_string_tx[94] = rxdata[76];
    spi_command_string_tx[95] = rxdata[77];
    spi_command_string_tx[96] = rxdata[78];
    spi_command_string_tx[97] = rxdata[79];
    spi_command_string_tx[98] = rxdata[80];
    spi_command_string_tx[99] = rxdata[81];
    spi_command_string_tx[100] = rxdata[82];
    spi_command_string_tx[101] = rxdata[83];
    spi_command_string_tx[102] = rxdata[84];
    spi_command_string_tx[103] = rxdata[85];
    spi_command_string_tx[104] = rxdata[86];
    spi_command_string_tx[105] = rxdata[87];
    spi_command_string_tx[106] = rxdata[88];
    spi_command_string_tx[107] = rxdata[89];
    spi_command_string_tx[108] = rxdata[90];
    spi_command_string_tx[109] = rxdata[91];
    spi_command_string_tx[110] = rxdata[92];
    spi_command_string_tx[111] = rxdata[93];
    spi_command_string_tx[112] = rxdata[94];
    spi_command_string_tx[113] = rxdata[95];
    spi_command_string_tx[114] = rxdata[96];
    spi_command_string_tx[115] = rxdata[97];
    spi_command_string_tx[116] = rxdata[98];
    spi_command_string_tx[117] = rxdata[99];
    spi_command_string_tx[118] = rxdata[100];
    spi_command_string_tx[119] = rxdata[101];
    spi_command_string_tx[120] = rxdata[102];
    spi_command_string_tx[121] = rxdata[103];
    spi_command_string_tx[122] = rxdata[104];
    spi_command_string_tx[123] = rxdata[105];
    spi_command_string_tx[124] = rxdata[106];
    spi_command_string_tx[125] = rxdata[107];
    spi_command_string_tx[126] = rxdata[108];
    spi_command_string_tx[127] = rxdata[109];
    spi_command_string_tx[128] = rxdata[110];
    spi_command_string_tx[129] = rxdata[111];
    spi_command_string_tx[130] = rxdata[112];
    spi_command_string_tx[131] = rxdata[113];
    spi_command_string_tx[132] = rxdata[114];
    spi_command_string_tx[133] = rxdata[115];
    spi_command_string_tx[134] = rxdata[116];
    spi_command_string_tx[135] = rxdata[117];
    spi_command_string_tx[136] = rxdata[118];
    spi_command_string_tx[137] = rxdata[119];
    spi_command_string_tx[138] = rxdata[120];
    spi_command_string_tx[139] = rxdata[121];
    spi_command_string_tx[140] = rxdata[122];
    spi_command_string_tx[141] = rxdata[123];
    spi_command_string_tx[142] = rxdata[124];
    spi_command_string_tx[143] = rxdata[125];
    spi_command_string_tx[144] = rxdata[126];
    spi_command_string_tx[145] = rxdata[127];

    spi_command_string_tx[146] = rxdata[128];
    spi_command_string_tx[147] = rxdata[129];
    spi_command_string_tx[148] = rxdata[130];
    spi_command_string_tx[149] = rxdata[131];
    spi_command_string_tx[150] = rxdata[132];
    spi_command_string_tx[151] = rxdata[133];
    spi_command_string_tx[152] = rxdata[134];
    spi_command_string_tx[153] = rxdata[135];
}

// void WriteFileData()
// {
//     //DAC 10
//     spi_command_string_tx[20] = 0x11; //A 2.25
//     spi_command_string_tx[21] = 0x33;
//     spi_command_string_tx[22] = 0x29; //B 17.65
//     spi_command_string_tx[23] = 0x6A;
//     spi_command_string_tx[24] = 0x33; //C 6.35
//     spi_command_string_tx[25] = 0x63;
//     spi_command_string_tx[26] = 0x41; //D 2.25
//     spi_command_string_tx[27] = 0x33;
//     //DAC 11
//     spi_command_string_tx[28] = rxdata[0]; //A 4.35
//     spi_command_string_tx[29] = rxdata[1];
//     spi_command_string_tx[30] = rxdata[2]; //B 10.46
//     spi_command_string_tx[31] = rxdata[3];
//     spi_command_string_tx[32] = 0x33; //C 6.35
//     spi_command_string_tx[33] = 0x63;
//     spi_command_string_tx[34] = 0x41; //D 2.25
//     spi_command_string_tx[35] = 0x33;
//     //DAC 12
//     spi_command_string_tx[36] = 0x11; //A 2.25
//     spi_command_string_tx[37] = 0x33;
//     spi_command_string_tx[38] = 37; //B 17.65
//     spi_command_string_tx[39] = 148;
//     spi_command_string_tx[40] = 0x33; //C 6.35
//     spi_command_string_tx[41] = 0x63;
//     spi_command_string_tx[42] = 0x41; //D 2.25
//     spi_command_string_tx[43] = 0x33;
//     //DAC 13
//     spi_command_string_tx[44] = 0x11; //A 2.25
//     spi_command_string_tx[45] = 0x33;
//     spi_command_string_tx[46] = 0x29; //B 17.65
//     spi_command_string_tx[47] = 0x6A;
//     spi_command_string_tx[48] = 0x33; //C 6.35
//     spi_command_string_tx[49] = 0x63;
//     spi_command_string_tx[50] = 0x41; //D 2.25
//     spi_command_string_tx[51] = 0x33;
//     //DAC 14
//     spi_command_string_tx[52] = 0x11; //A 2.25
//     spi_command_string_tx[53] = 0x33;
//     spi_command_string_tx[54] = 0x29; //B 17.65
//     spi_command_string_tx[55] = 0x6A;
//     spi_command_string_tx[56] = 0x33; //C 6.35
//     spi_command_string_tx[57] = 0x63;
//     spi_command_string_tx[58] = 0x41; //D 2.25
//     spi_command_string_tx[59] = 0x33;
//     //DAC 15
//     spi_command_string_tx[60] = 0x11; //A 2.25
//     spi_command_string_tx[61] = 0x33;
//     spi_command_string_tx[62] = 0x29; //B 17.65
//     spi_command_string_tx[63] = 0x6A;
//     spi_command_string_tx[64] = 0x33; //C 6.35
//     spi_command_string_tx[65] = 0x63;
//     spi_command_string_tx[66] = 0x41; //D 2.25
//     spi_command_string_tx[67] = 0x33;
//     //DAC 16
//     spi_command_string_tx[68] = 0x11; //A 2.25
//     spi_command_string_tx[69] = 0x33;
//     spi_command_string_tx[70] = 0x29; //B 17.65
//     spi_command_string_tx[71] = 0x6A;
//     spi_command_string_tx[72] = 0x33; //C 6.35
//     spi_command_string_tx[73] = 0x63;
//     spi_command_string_tx[74] = 0x41; //D 2.25
//     spi_command_string_tx[75] = 0x33;
//     //DAC 17
//     spi_command_string_tx[76] = 0x11; //A 2.25
//     spi_command_string_tx[77] = 0x33;
//     spi_command_string_tx[78] = 0x29; //B 17.65
//     spi_command_string_tx[79] = 0x6A;
//     spi_command_string_tx[80] = 0x33; //C 6.35
//     spi_command_string_tx[81] = 0x63;
//     spi_command_string_tx[82] = 0x41; //D 2.25
//     spi_command_string_tx[83] = 0x33;
//     //DAC 18
//     spi_command_string_tx[84] = 0x11; //A 2.25
//     spi_command_string_tx[85] = 0x33;
//     spi_command_string_tx[86] = 0x29; //B 17.65
//     spi_command_string_tx[87] = 0x6A;
//     spi_command_string_tx[88] = 0x33; //C 6.35
//     spi_command_string_tx[89] = 0x63;
//     spi_command_string_tx[90] = 0x41; //D 2.25
//     spi_command_string_tx[91] = 0x33;
//     //DAC 19
//     spi_command_string_tx[92] = 0x11; //A 2.25
//     spi_command_string_tx[93] = 0x33;
//     spi_command_string_tx[94] = 0x29; //B 17.65
//     spi_command_string_tx[95] = 0x6A;
//     spi_command_string_tx[96] = 0x33; //C 6.35
//     spi_command_string_tx[97] = 0x63;
//     spi_command_string_tx[98] = 0x41; //D 2.25
//     spi_command_string_tx[99] = 0x33;
//     //DAC 20
//     spi_command_string_tx[100] = 0x11; //A 2.25
//     spi_command_string_tx[101] = 0x33;
//     spi_command_string_tx[102] = 0x29; //B 17.65
//     spi_command_string_tx[103] = 0x6A;
//     spi_command_string_tx[104] = 0x33; //C 6.35
//     spi_command_string_tx[105] = 0x63;
//     spi_command_string_tx[106] = 0x41; //D 2.25
//     spi_command_string_tx[107] = 0x33;
//     //DAC 21
//     spi_command_string_tx[108] = 0x11; //A 2.25
//     spi_command_string_tx[109] = 0x33;
//     spi_command_string_tx[110] = 0x29; //B 17.65
//     spi_command_string_tx[111] = 0x6A;
//     spi_command_string_tx[112] = 0x33; //C 6.35
//     spi_command_string_tx[113] = 0x63;
//     spi_command_string_tx[114] = 0x41; //D 2.25
//     spi_command_string_tx[115] = 0x33;
//     //DAC 22
//     spi_command_string_tx[116] = 0x11; //A 2.25
//     spi_command_string_tx[117] = 0x33;
//     spi_command_string_tx[118] = 0x29; //B 17.65
//     spi_command_string_tx[119] = 0x6A;
//     spi_command_string_tx[120] = 0x33; //C 6.35
//     spi_command_string_tx[121] = 0x63;
//     spi_command_string_tx[122] = 0x41; //D 2.25
//     spi_command_string_tx[123] = 0x33;
//     //DAC 23
//     spi_command_string_tx[124] = 0x11; //A 2.25
//     spi_command_string_tx[125] = 0x33;
//     spi_command_string_tx[126] = 0x29; //B 17.65
//     spi_command_string_tx[127] = 0x6A;
//     spi_command_string_tx[128] = 0x33; //C 6.35
//     spi_command_string_tx[129] = 0x63;
//     spi_command_string_tx[130] = 0x41; //D 2.25
//     spi_command_string_tx[131] = 0x33;
//     //DAC 24
//     spi_command_string_tx[132] = 0x11; //A 2.25
//     spi_command_string_tx[133] = 0x33;
//     spi_command_string_tx[134] = 0x29; //B 17.65
//     spi_command_string_tx[135] = 0x6A;
//     spi_command_string_tx[136] = 0x33; //C 6.35
//     spi_command_string_tx[137] = 0x63;
//     spi_command_string_tx[138] = 0x41; //D 2.25
//     spi_command_string_tx[139] = 0x33;
//     //DAC 25
//     spi_command_string_tx[140] = 0x11; //A 2.25
//     spi_command_string_tx[141] = 0x33;
//     spi_command_string_tx[142] = 0x29; //B 17.65
//     spi_command_string_tx[143] = 0x6A;
//     spi_command_string_tx[144] = 0x33; //C 6.35
//     spi_command_string_tx[145] = 0x63;
//     spi_command_string_tx[146] = 0x41; //D 2.25
//     spi_command_string_tx[147] = 0x33;
// }
