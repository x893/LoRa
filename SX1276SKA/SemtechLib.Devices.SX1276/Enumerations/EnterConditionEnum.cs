namespace SemtechLib.Devices.SX1276.Enumerations
{
	public enum EnterConditionEnum
	{
		OFF,
		RisingEdgeFifoNotEmpty,
		RisingEdgeFifoLevel,
		RisingEdgeCrcOk,
		RisingEdgePayloadReady,
		RisingEdgeSyncAddress,
		RisingEdgePacketSent,
		FallingEdgeFifoNotEmpty,
	}
}
