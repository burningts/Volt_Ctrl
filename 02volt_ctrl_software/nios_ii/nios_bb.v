
module nios (
	clk_clk,
	epcs_flash_dclk,
	epcs_flash_sce,
	epcs_flash_sdo,
	epcs_flash_data0,
	ldac_n_export,
	oe_export,
	pio_led_export,
	reset_reset_n,
	spi_MISO,
	spi_MOSI,
	spi_SCLK,
	spi_SS_n,
	uart_rxd,
	uart_txd);	

	input		clk_clk;
	output		epcs_flash_dclk;
	output		epcs_flash_sce;
	output		epcs_flash_sdo;
	input		epcs_flash_data0;
	output		ldac_n_export;
	output	[31:0]	oe_export;
	output		pio_led_export;
	input		reset_reset_n;
	input		spi_MISO;
	output		spi_MOSI;
	output		spi_SCLK;
	output		spi_SS_n;
	input		uart_rxd;
	output		uart_txd;
endmodule
