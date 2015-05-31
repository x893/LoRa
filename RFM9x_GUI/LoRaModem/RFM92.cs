namespace LoRaModem
{
	public class RFM92
	{
		public RegType RegFifo = new RegType("RegFifo", (byte)0, (byte)0);
		public RegType RegOpMode = new RegType("RegOpMode", (byte)1, (byte)1);
		public RegType RegBitrateMsb = new RegType("RegBitrateMsb", (byte)2, (byte)208);
		public RegType RegBitrateLsb = new RegType("RegBitrateLsb", (byte)3, (byte)85);
		public RegType RegFdevMsb = new RegType("RegFdevMsb", (byte)4, (byte)1);
		public RegType RegFdevLsb = new RegType("RegFdevLsb", (byte)5, (byte)39);
		public RegType RegFrMsb = new RegType("RegFrMsb", (byte)6, (byte)228);
		public RegType RegFrMid = new RegType("RegFrMid", (byte)7, (byte)192);
		public RegType RegFrLsb = new RegType("RegFrLsb", (byte)8, (byte)0);
		public RegType RegPaConfig = new RegType("RegPaConfig", (byte)9, (byte)15);
		public RegType RegPaRamp = new RegType("RegPaRamp", (byte)10, (byte)25);
		public RegType RegOcp = new RegType("RegOcp", (byte)11, (byte)43);
		public RegType RegLna = new RegType("RegLna", (byte)12, (byte)32);
		public RegType RegFifoAddrPtr = new RegType("RegFifoAddrPtr", (byte)13, (byte)0);
		public RegType RegFifoTxBaseAddr = new RegType("RegFifoTxBaseAddr", (byte)14, 0x80);
		public RegType RegFifoRxBaseAddr = new RegType("RegFifoRxBaseAddr", (byte)15, (byte)0);
		public RegType RegFifoRxCurrentAddr = new RegType("RegFifoRxCurrentAddr", (byte)16, (byte)0);
		public RegType RegIrqFlagsMask = new RegType("RegIrqFlagsMask", (byte)17, (byte)0);
		public RegType RegIrqFlags = new RegType("RegIrqFlags", (byte)18, (byte)0);
		public RegType RegRxNbBytes = new RegType("RegRxNbBytes", (byte)19, (byte)0);
		public RegType RegRxHeaderCntValueMsb = new RegType("RegRxHeaderCntValueMsb", (byte)20, (byte)0);
		public RegType RegRxHeaderCntValueLsb = new RegType("RegRxHeaderCntValueLsb", (byte)21, (byte)0);
		public RegType RegRxPacketCntValueMsb = new RegType("RegRxPacketCntValueMsb", (byte)22, (byte)0);
		public RegType RegRxPacketCntValueLsb = new RegType("RegRxPacketCntValueLsb", (byte)23, (byte)0);
		public RegType RegModemStat = new RegType("RegModemStat", (byte)24, (byte)16);
		public RegType RegPktSnrValue = new RegType("RegPktSnrValue", (byte)25, (byte)0);
		public RegType RegPktRssiValue = new RegType("RegPktRssiValue", (byte)26, (byte)0);
		public RegType RegRssiValue = new RegType("RegRssiValue", (byte)27, (byte)0);
		public RegType RegHopChannel = new RegType("RegHopChannel", (byte)28, (byte)0);
		public RegType RegModemConfig1 = new RegType("RegModemConfig1", (byte)29, (byte)8);
		public RegType RegModemConfig2 = new RegType("RegModemConfig2", (byte)30, (byte)116);
		public RegType RegSymbTimeoutLsb = new RegType("RegSymbTimeoutLsb", (byte)31, (byte)100);
		public RegType RegPreambleMsb = new RegType("RegPreambleMsb", (byte)32, (byte)0);
		public RegType RegPreambleLsb = new RegType("RegPreambleLsb", (byte)33, (byte)8);
		public RegType RegPayloadLength = new RegType("RegPayloadLength", (byte)34, (byte)1);
		public RegType RegMaxPayloadLength = new RegType("RegMaxPayloadLength", (byte)35, byte.MaxValue);
		public RegType RegHopPeriod = new RegType("RegHopPeriod", (byte)36, (byte)0);
		public RegType RegFifoRxByteAddr = new RegType("RegFifoRxByteAddr", (byte)37, (byte)0);
		public RegType RegVersion = new RegType("RegVersion", (byte)66, (byte)34);
		public RegType RegTcxo = new RegType("RegTcxo", (byte)88, (byte)9);
		public RegType RegPaDac = new RegType("RegPaDac", (byte)90, (byte)132);
		public RegType RegPllHf = new RegType("RegPllHf", (byte)92, (byte)208);

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
