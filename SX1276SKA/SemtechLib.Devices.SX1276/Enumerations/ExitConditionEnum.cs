namespace SemtechLib.Devices.SX1276.Enumerations
{
	public enum ExitConditionEnum
	{
		OFF,
		FallingEdgeFifoNotEmpty,
		RisingEdgeFifoLevel,
		RisingEdgeCrcOk,
		RisingEdgePayloadReadyOrTimeout,
		RisingEdgeSyncAddressOrTimeout,
		RisingEdgePacketSent,
		RisingEdgeTimeout,
	}
}
