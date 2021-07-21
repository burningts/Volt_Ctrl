	component nios is
		port (
			clk_clk          : in  std_logic                     := 'X'; -- clk
			ldac_n_export    : out std_logic;                            -- export
			oe_export        : out std_logic_vector(31 downto 0);        -- export
			pio_led_export   : out std_logic;                            -- export
			reset_reset_n    : in  std_logic                     := 'X'; -- reset_n
			spi_MISO         : in  std_logic                     := 'X'; -- MISO
			spi_MOSI         : out std_logic;                            -- MOSI
			spi_SCLK         : out std_logic;                            -- SCLK
			spi_SS_n         : out std_logic;                            -- SS_n
			uart_rxd         : in  std_logic                     := 'X'; -- rxd
			uart_txd         : out std_logic;                            -- txd
			epcs_flash_dclk  : out std_logic;                            -- dclk
			epcs_flash_sce   : out std_logic;                            -- sce
			epcs_flash_sdo   : out std_logic;                            -- sdo
			epcs_flash_data0 : in  std_logic                     := 'X'  -- data0
		);
	end component nios;

	u0 : component nios
		port map (
			clk_clk          => CONNECTED_TO_clk_clk,          --        clk.clk
			ldac_n_export    => CONNECTED_TO_ldac_n_export,    --     ldac_n.export
			oe_export        => CONNECTED_TO_oe_export,        --         oe.export
			pio_led_export   => CONNECTED_TO_pio_led_export,   --    pio_led.export
			reset_reset_n    => CONNECTED_TO_reset_reset_n,    --      reset.reset_n
			spi_MISO         => CONNECTED_TO_spi_MISO,         --        spi.MISO
			spi_MOSI         => CONNECTED_TO_spi_MOSI,         --           .MOSI
			spi_SCLK         => CONNECTED_TO_spi_SCLK,         --           .SCLK
			spi_SS_n         => CONNECTED_TO_spi_SS_n,         --           .SS_n
			uart_rxd         => CONNECTED_TO_uart_rxd,         --       uart.rxd
			uart_txd         => CONNECTED_TO_uart_txd,         --           .txd
			epcs_flash_dclk  => CONNECTED_TO_epcs_flash_dclk,  -- epcs_flash.dclk
			epcs_flash_sce   => CONNECTED_TO_epcs_flash_sce,   --           .sce
			epcs_flash_sdo   => CONNECTED_TO_epcs_flash_sdo,   --           .sdo
			epcs_flash_data0 => CONNECTED_TO_epcs_flash_data0  --           .data0
		);

