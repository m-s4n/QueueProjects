using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExcelCreator.Web.Entities
{
    public class UserFileConfig : IEntityTypeConfiguration<UserFile>
    {
        public void Configure(EntityTypeBuilder<UserFile> builder)
        {
            builder.ToTable("user_file").HasKey("Id");
            builder.Property(x => x.Id).HasColumnType("integer").HasColumnName("id");
            builder.Property(x => x.UserId).HasColumnType("varchar(250)").HasColumnName("user_id");
            builder.Property(x => x.FileName).HasColumnType("varchar(500)").HasColumnName("file_name");
            builder.Property(x => x.FilePath).HasColumnType("varchar(500)").HasColumnName("file_path");
            builder.Property(x => x.CreatedDate).HasColumnType("timestamp without time zone").HasColumnName("created_date");
            builder.Property(x => x.FileStatus).HasColumnType("integer").HasColumnName("file_status");

            builder.Ignore(x => x.GetCreateDate);
        }
    }
}
