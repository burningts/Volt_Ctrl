module volt_ctrl (
    input sys_clk_50m,
    input sys_rst_n,
    input key,
    
    input  spi_MISO,
    output spi_MOSI,
    output spi_SS_n,
    output spi_SCLK, 
    output ldac_n,
    output clr_n,
    output [31:0] oe_n, 
    input  uart_rxd, 
    output uart_txd,
    output pio_led,

    output epcs_flash_dclk , // EPCS FLASF的驱动时钟 
    output epcs_flash_sce , // 片选信号 
    output epcs_flash_sdo , // 数据输出
    input epcs_flash_data0, // 数据输入    
    output led               );

wire rst_n;
wire locked;
reg led_reg;
reg clr_n_reg;
wire clk_100m;

pll	u_pll (
	.areset ( ~sys_rst_n ),
	.inclk0 ( sys_clk_50m ),
	.c0     ( clk_100m ),
	.locked ( locked )
	);


assign rst_n = (sys_rst_n);


nios u_nios (
    .clk_clk       (clk_100m),      //   clk.clk
    .reset_reset_n (rst_n),         // reset.reset_n
    .spi_MISO      (spi_MISO),      //   spi.MISO
    .spi_MOSI      (spi_MOSI),      //      .MOSIs
    .spi_SCLK      (spi_SCLK),      //      .SCLK
    .spi_SS_n      (spi_SS_n),       //      .SS_n
    .oe_export     (oe_n),
    .ldac_n_export (ldac_n),  // ldac_n.export
    .uart_rxd      (uart_rxd),      //   uart.rxd
    .uart_txd      (uart_txd),       //       .txd
    .pio_led_export (pio_led),  // pio_led.export
    .epcs_flash_dclk  (epcs_flash_dclk),  // epcs_flash.dclk
    .epcs_flash_sce   (epcs_flash_sce),   //           .sce
    .epcs_flash_sdo   (epcs_flash_sdo),   //           .sdo
    .epcs_flash_data0 (epcs_flash_data0)  //           .data0

);


// assign oe11_n = spi_SS_n; 
// assign oe12_n = 1'd1; 
// always @(posedge sys_clk_50m or negedge rst_n) begin
//     if(!rst_n)begin
//       ldac_cnt <= 10'd0;
//     end
//     else begin
//       if ((!spi_SS_n)&&(ldac_cnt <= 10'd1000)) begin
//           ldac_cnt <= ldac_cnt + 10'd1;
//       end
//       else begin
//         ldac_cnt <= 10'd0;
//       end
//     end
// end

//spi speed:1MHz,sys_clk 50MHz
//when spi_SS_n turn to 0,ldac_n should turn to 0.
assign clr_n = clr_n_reg;
always @(posedge sys_clk_50m or negedge rst_n) begin
    if(!rst_n)begin
      clr_n_reg <= 1'd0;
    end 
    else begin
       clr_n_reg <= 1'd1;
    end 
end

assign led = led_reg;

always @(posedge sys_clk_50m or negedge rst_n) begin
    if (!rst_n) begin
        led_reg <= 1'd0; 
    end
    else begin
        if (!key) begin
           led_reg <= 1'd1;
        end
        else begin
            led_reg <= 1'd0;
        end         
    end
end
endmodule