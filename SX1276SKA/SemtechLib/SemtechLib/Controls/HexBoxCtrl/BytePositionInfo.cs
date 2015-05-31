namespace SemtechLib.Controls.HexBoxCtrl
{
	internal struct BytePositionInfo
	{
		private int _characterPosition;
		private long _index;

		public int CharacterPosition
		{
			get
			{
				return _characterPosition;
			}
		}

		public long Index
		{
			get
			{
				return _index;
			}
		}

		public BytePositionInfo(long index, int characterPosition)
		{
			_index = index;
			_characterPosition = characterPosition;
		}
	}
}
