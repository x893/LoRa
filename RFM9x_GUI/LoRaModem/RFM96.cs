namespace LoRaModem
{
	public class RFM96
	{
		public RegType RegFifo = new RegType("RegFifo", 0, 0);
		public RegType RegOpMode = new RegType("RegOpMode", 1, 1);
		public RegType RegBitrateMsb = new RegType("RegBitrateMsb", 2, 208);
		public RegType RegBitrateLsb = new RegType("RegBitrateLsb", 3, 85);
		public RegType RegFdevMsb = new RegType("RegFdevMsb", 4, 1);
		public RegType RegFdevLsb = new RegType("RegFdevLsb", 5, 39);
		public RegType RegFrMsb = new RegType("RegFrMsb", 6, 108);
		public RegType RegFrMid = new RegType("RegFrMid", 7, 0x80);
		public RegType RegFrLsb = new RegType("RegFrLsb", 8, 0);
		public RegType RegPaConfig = new RegType("RegPaConfig", 9, 79);
		public RegType RegPaRamp = new RegType("RegPaRamp", 10, 9);
		public RegType RegOcp = new RegType("RegOcp", 11, 43);
		public RegType RegLna = new RegType("RegLna", 12, 32);
		public RegType RegFifoAddrPtr = new RegType("RegFifoAddrPtr", 13, 0);
		public RegType RegFifoTxBaseAddr = new RegType("RegFifoTxBaseAddr", 14, 0x80);
		public RegType RegFifoRxBaseAddr = new RegType("RegFifoRxBaseAddr", 15, 0);
		public RegType RegFifoRxCurrentAddr = new RegType("RegFifoRxCurrentAddr", 16, 0);
		public RegType RegIrqFlagsMask = new RegType("RegIrqFlagsMask", 17, 0);
		public RegType RegIrqFlags = new RegType("RegIrqFlags", 18, 0);
		public RegType RegRxNbBytes = new RegType("RegRxNbBytes", 19, 0);
		public RegType RegRxHeaderCntValueMsb = new RegType("RegRxHeaderCntValueMsb", 20, 0);
		public RegType RegRxHeaderCntValueLsb = new RegType("RegRxHeaderCntValueLsb", 21, 0);
		public RegType RegRxPacketCntValueMsb = new RegType("RegRxPacketCntValueMsb", 22, 0);
		public RegType RegRxPacketCntValueLsb = new RegType("RegRxPacketCntValueLsb", 23, 0);
		public RegType RegModemStat = new RegType("RegModemStat", 24, 16);
		public RegType RegPktSnrValue = new RegType("RegPktSnrValue", 25, 0);
		public RegType RegPktRssiValue = new RegType("RegPktRssiValue", 26, 0);
		public RegType RegRssiValue = new RegType("RegRssiValue", 27, 0);
		public RegType RegHopChannel = new RegType("RegHopChannel", 28, 0);
		public RegType RegModemConfig1 = new RegType("RegModemConfig1", 29, 114);
		public RegType RegModemConfig2 = new RegType("RegModemConfig2", 30, 112);
		public RegType RegSymbTimeoutLsb = new RegType("RegSymbTimeoutLsb", 31, 100);
		public RegType RegPreambleMsb = new RegType("RegPreambleMsb", 32, 0);
		public RegType RegPreambleLsb = new RegType("RegPreambleLsb", 33, 8);
		public RegType RegPayloadLength = new RegType("RegPayloadLength", 34, 1);
		public RegType RegMaxPayloadLength = new RegType("RegMaxPayloadLength", 35, byte.MaxValue);
		public RegType RegHopPeriod = new RegType("RegHopPeriod", 36, 0);
		public RegType RegFifoRxByteAddr = new RegType("RegFifoRxByteAddr", 37, 0);
		public RegType RegModemConfig3 = new RegType("RegModemConfig3", 38, 0);
		public RegType RegVersion = new RegType("RegVersion", 66, 18);
		public RegType RegTcxo = new RegType("RegTcxo", 75, 9);
		public RegType RegPaDac = new RegType("RegPaDac", 77, 132);
		public RegType RegPllHf = new RegType("RegPllHf", 112, 208);

		public void ApplyValue()
		{
			RegFrMsb.ApplyValue();
			RegFrMid.ApplyValue();
			RegFrLsb.ApplyValue();
			RegPaConfig.ApplyValue();
			RegPaRamp.ApplyValue();
			RegOcp.ApplyValue();
			RegLna.ApplyValue();
			RegFifoAddrPtr.ApplyValue();
			RegFifoRxBaseAddr.ApplyValue();
			RegFifoTxBaseAddr.ApplyValue();
			RegFifoRxCurrentAddr.ApplyValue();
			RegIrqFlagsMask.ApplyValue();
			RegIrqFlags.ApplyValue();
			RegRxNbBytes.ApplyValue();
			RegModemConfig1.ApplyValue();
			RegModemConfig2.ApplyValue();
			RegModemConfig3.ApplyValue();
			RegSymbTimeoutLsb.ApplyValue();
			RegPreambleMsb.ApplyValue();
			RegPreambleLsb.ApplyValue();
			RegPayloadLength.ApplyValue();
			RegMaxPayloadLength.ApplyValue();
			RegHopPeriod.ApplyValue();
			RegPaDac.ApplyValue();
			RegPllHf.ApplyValue();
			RegTcxo.ApplyValue();
		}
	}
}
