using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DMI.SMS.Domain.Entities;

namespace DMI.SMS.Persistence.EntityFrameworkCore.EntityConfigurations
{
    public class StationInformationConfiguration : IEntityTypeConfiguration<StationInformation>
    {
        public void Configure(EntityTypeBuilder<StationInformation> builder)
        {
            builder.HasKey(_ => _.GdbArchiveOid);

            /*
            builder.Property(si => si.GdbArchiveOid).HasColumnName("gdb_archive_oid");
            builder.Property(si => si.GlobalId).HasColumnName("globalid");
            builder.Property(si => si.ObjectId).HasColumnName("objectid");
            builder.Property(si => si.CreatedUser).HasColumnName("created_user");
            builder.Property(si => si.CreatedDate).HasColumnName("created_date");
            builder.Property(si => si.LastEditedUser).HasColumnName("last_edited_user");
            builder.Property(si => si.LastEditedDate).HasColumnName("last_edited_date");
            builder.Property(si => si.GdbFromDate).HasColumnName("gdb_from_date");
            builder.Property(si => si.GdbToDate).HasColumnName("gdb_to_date");

            builder.Property(si => si.StationName).HasColumnName("stationname");
            builder.Property(si => si.StationIDDMI).HasColumnName("stationid_dmi");
            builder.Property(si => si.Stationtype).HasColumnName("stationtype");
            builder.Property(si => si.AccessAddress).HasColumnName("accessaddress");
            builder.Property(si => si.Country).HasColumnName("country");
            builder.Property(si => si.Status).HasColumnName("status");
            builder.Property(si => si.DateFrom).HasColumnName("datefrom");
            builder.Property(si => si.DateTo).HasColumnName("dateto");
            builder.Property(si => si.StationOwner).HasColumnName("stationOwner");
            builder.Property(si => si.Comment).HasColumnName("comment");
            builder.Property(si => si.Stationid_icao).HasColumnName("stationid_icao");
            builder.Property(si => si.Referencetomaintenanceagreement).HasColumnName("referencetomaintenanceagreement");
            builder.Property(si => si.Facilityid).HasColumnName("facilityid");
            builder.Property(si => si.Si_utm).HasColumnName("si_utm");
            builder.Property(si => si.Si_northing).HasColumnName("si_northing");
            builder.Property(si => si.Si_easting).HasColumnName("si_easting");
            builder.Property(si => si.Si_geo_lat).HasColumnName("si_geo_lat");
            builder.Property(si => si.Si_geo_long).HasColumnName("si_geo_long");
            builder.Property(si => si.Serviceinterval).HasColumnName("serviceinterval");
            builder.Property(si => si.Lastservicedate).HasColumnName("lastservicedate");
            builder.Property(si => si.Nextservicedate).HasColumnName("nextservicedate");
            builder.Property(si => si.Addworkforcedate).HasColumnName("addworkforcedate");
            builder.Property(si => si.Lastvisitdate).HasColumnName("lastvisitdate");
            builder.Property(si => si.Altstationid).HasColumnName("altstationid");
            builder.Property(si => si.Wmostationid).HasColumnName("wmostationid");
            builder.Property(si => si.Regionid).HasColumnName("regionid");
            builder.Property(si => si.Wigosid).HasColumnName("wigosid");
            builder.Property(si => si.Wmocountrycode).HasColumnName("wmocountrycode");
            builder.Property(si => si.Hha).HasColumnName("hha");
            builder.Property(si => si.Hhp).HasColumnName("hhp");
            builder.Property(si => si.Wmorbsn).HasColumnName("wmorbsn");
            builder.Property(si => si.Wmorbcn).HasColumnName("wmorbcn");
            builder.Property(si => si.Wmorbsnradio).HasColumnName("wmorbsnradio");
            builder.Property(si => si.Wgs_lat).HasColumnName("wgs_lat");
            builder.Property(si => si.Wgs_long).HasColumnName("wgs_long");
            */

            /*
            Property(si => si.StationName)
                .IsOptional()
                .HasMaxLength(500);

            Property(si => si.Stationtype)
                .IsOptional();

            Property(si => si.Country)
                .IsOptional();

            Property(si => si.Status)
                .IsOptional();

            Property(si => si.StationOwner)
                .IsOptional();

            Property(si => si.AccessAddress)
                .IsOptional()
                .HasMaxLength(500);

            Property(si => si.Comment)
                .IsOptional()
                .HasMaxLength(500);

            Property(si => si.Stationid_icao)
                .IsOptional()
                .HasMaxLength(255);

            Property(si => si.Referencetomaintenanceagreement)
                .IsOptional()
                .HasMaxLength(500);

            Property(si => si.Facilityid)
                .IsOptional()
                .HasMaxLength(500);

            Property(si => si.CreatedUser)
                .IsOptional()
                .HasMaxLength(255);

            Property(si => si.LastEditedUser)
                .IsOptional()
                .HasMaxLength(255);

            Property(si => si.Altstationid)
                .IsOptional()
                .HasMaxLength(10);

            Property(si => si.Wmostationid)
                .IsOptional()
                .HasMaxLength(20);

            Property(si => si.Regionid)
                .IsOptional()
                .HasMaxLength(10);

            Property(si => si.Wigosid)
                .IsOptional()
                .HasMaxLength(15);

            Property(si => si.Wmocountrycode)
                .IsOptional()
                .HasMaxLength(10);

            Property(si => si.GlobalId)
                .IsRequired()
                .HasMaxLength(38);
            */
        }
    }
}
