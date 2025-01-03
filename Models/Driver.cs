using iRacingSdkWrapper;

namespace SharpOverlay.Models
{
    public class Driver
    {
        public Driver()
        {
        }

        /// <summary>
        /// The identifier (CarIdx) of this driver (unique to this session)
        /// </summary>
        public int CarIdx { get; set; }

        /// <summary>
        /// The current position of the driver
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// The name of the driver
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The car number of this driver
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Used to determine if a driver is in the pits, off or on track
        /// </summary>
        public TrackSurfaces TrackSurface { get; set; }


        /// <summary>
        /// The lap this driver is currently in
        /// </summary>
        public int Lap { get; set; }

        /// <summary>
        /// The distance along the current lap of this driver (in percentage)
        /// </summary>
        public float LapDistancePct { get; set; }

        /// <summary>
        /// The relative distance between you and this driver (in percentage).
        /// </summary>
        public float RelativeLapDistancePct { get; set; }

    }
}
