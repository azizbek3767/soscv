// Soskd.Infrastructure/Data/ApplicationDbContext.cs
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soskd.Domain.Entities;
using Soskd.Domain.Enums;

namespace Soskd.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<News> News { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<MediaAboutUs> MediaAboutUs { get; set; }
        public DbSet<Document> Documents { get; set; } // NEW
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<VacancyApplication> VacancyApplications { get; set; } // NEW
        public DbSet<ContactApplication> ContactApplications { get; set; }
        public DbSet<Donation> Donations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure News entity
            builder.Entity<News>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.TitleUz).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleRu).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(500);

                entity.Property(e => e.DescriptionUz).IsRequired();
                entity.Property(e => e.DescriptionRu).IsRequired();
                entity.Property(e => e.DescriptionEn).IsRequired();

                // Slugs
                entity.Property(e => e.SlugUz).HasMaxLength(500);
                entity.Property(e => e.SlugRu).HasMaxLength(500);
                entity.Property(e => e.SlugEn).HasMaxLength(500);

                // Meta Titles
                entity.Property(e => e.MetaTitleUz).HasMaxLength(500);
                entity.Property(e => e.MetaTitleRu).HasMaxLength(500);
                entity.Property(e => e.MetaTitleEn).HasMaxLength(500);

                // Meta Descriptions
                entity.Property(e => e.MetaDescriptionUz).HasMaxLength(1000);
                entity.Property(e => e.MetaDescriptionRu).HasMaxLength(1000);
                entity.Property(e => e.MetaDescriptionEn).HasMaxLength(1000);

                entity.Property(e => e.SmallPhotoUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.LargePhotoUrl).HasMaxLength(500);

                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.PublishedDate).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            // Configure Vacancy entity
            builder.Entity<Vacancy>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Titles
                entity.Property(e => e.TitleUz).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleRu).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(500);

                // Descriptions
                entity.Property(e => e.DescriptionUz).IsRequired();
                entity.Property(e => e.DescriptionRu).IsRequired();
                entity.Property(e => e.DescriptionEn).IsRequired();

                // Status
                entity.Property(e => e.Status).HasConversion<int>();

                // Optional dates
                entity.Property(e => e.PublishedDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.Deadline).HasColumnType("timestamp with time zone");

                // Audit fields
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("timestamp with time zone");

                // Indexes for better query performance
                entity.HasIndex(e => e.PublishedDate);
                entity.HasIndex(e => e.Deadline);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Status);
            });

            builder.Entity<MediaAboutUs>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Titles
                entity.Property(e => e.TitleUz).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleRu).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(500);

                // Descriptions
                entity.Property(e => e.DescriptionUz).IsRequired();
                entity.Property(e => e.DescriptionRu).IsRequired();
                entity.Property(e => e.DescriptionEn).IsRequired();

                // Photo and Source
                entity.Property(e => e.PhotoUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.SourceLink).IsRequired().HasMaxLength(1000);

                // Status
                entity.Property(e => e.Status).HasConversion<int>();

                // Dates
                entity.Property(e => e.PublishDate).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("timestamp with time zone");

                // Indexes for better query performance
                entity.HasIndex(e => e.PublishDate);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Status);
            });

            builder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Titles
                entity.Property(e => e.TitleUz).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleRu).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(500);

                // Descriptions (Optional)
                entity.Property(e => e.DescriptionUz);
                entity.Property(e => e.DescriptionRu);
                entity.Property(e => e.DescriptionEn);

                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - Made file information optional in database
                // File information - Russian (original) - OPTIONAL
                entity.Property(e => e.FileUrl).HasMaxLength(500).IsRequired(false);
                entity.Property(e => e.FileName).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.FileSizeBytes).IsRequired(false);

                // File information - Uzbek - OPTIONAL
                entity.Property(e => e.FileUrlUz).HasMaxLength(500).IsRequired(false);
                entity.Property(e => e.FileNameUz).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.FileSizeBytesUz).IsRequired(false);

                // File information - English - OPTIONAL
                entity.Property(e => e.FileUrlEn).HasMaxLength(500).IsRequired(false);
                entity.Property(e => e.FileNameEn).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.FileSizeBytesEn).IsRequired(false);
                // DDDDDDDDDDDDDDDDDDDDDDDDDDDDD - End of optional file configuration

                // Category translations
                entity.Property(e => e.CategoryUz).HasMaxLength(100);
                entity.Property(e => e.CategoryRu).HasMaxLength(100);
                entity.Property(e => e.CategoryEn).HasMaxLength(100);

                // Category and Status
                entity.Property(e => e.Category).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();

                // Dates
                entity.Property(e => e.PublishDate).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("timestamp with time zone");

                // Indexes for better query performance
                entity.HasIndex(e => e.PublishDate);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Category);
            });
            builder.Entity<Sponsor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Titles
                entity.Property(e => e.TitleUz).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TitleRu).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TitleEn).IsRequired().HasMaxLength(200);

                // Photo and Link
                entity.Property(e => e.PhotoUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Link).IsRequired().HasMaxLength(500);

                // Status and Display Order
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.DisplayOrder).IsRequired();

                // Audit fields
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("timestamp with time zone");

                // Indexes for better query performance
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });
            builder.Entity<VacancyApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Personal Information
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);

                // Application Details
                entity.Property(e => e.CoverLetter).IsRequired();
                entity.Property(e => e.ResumeUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ResumeFileName).HasMaxLength(100);

                // Vacancy Reference
                entity.Property(e => e.VacancyId).IsRequired();
                entity.Property(e => e.VacancyTitle).IsRequired().HasMaxLength(500);

                // Audit Fields
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");

                // Foreign Key Relationship
                entity.HasOne(e => e.Vacancy)
                      .WithMany()
                      .HasForeignKey(e => e.VacancyId)
                      .OnDelete(DeleteBehavior.Restrict); // Don't delete applications if vacancy is deleted

                // Indexes for better query performance
                entity.HasIndex(e => e.VacancyId);
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.CreatedAt);
            });

            builder.Entity<ContactApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Personal Information
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.City).IsRequired().HasMaxLength(100);

                // Message
                entity.Property(e => e.Message).IsRequired();

                // Audit and tracking fields
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);

                // Indexes for better query performance
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.City);
            });
            builder.Entity<Donation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Personal Information
                entity.Property(e => e.DonorName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DonorEmail).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DonorPhone).HasMaxLength(20);

                // Donation Details
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProcessedAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Type).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();

                // Transaction Details
                entity.Property(e => e.OrderId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UpayTransactionId).HasMaxLength(100);
                entity.Property(e => e.PaymentMethod).HasMaxLength(500);
                entity.Property(e => e.FailureReason).HasMaxLength(1000);

                // Technical Details
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);

                // Audit Fields
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("timestamp with time zone");
                entity.Property(e => e.PaymentCompletedAt).HasColumnType("timestamp with time zone");
                entity.Property(e => e.NextPaymentDate).HasColumnType("timestamp with time zone");

                // Self-referencing relationship for recurring payments
                entity.HasOne(e => e.ParentDonation)
                      .WithMany(e => e.RecurringPayments)
                      .HasForeignKey(e => e.ParentDonationId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Indexes for better query performance
                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.UpayTransactionId);
                entity.HasIndex(e => e.DonorEmail);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.NextPaymentDate);
            });

            // Seed data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Use a fixed UTC base time for consistent seed data
            var baseUtcTime = new DateTime(2024, 12, 1, 10, 0, 0, DateTimeKind.Utc);

            // Seed Admin User
            var adminUser = new IdentityUser
            {
                Id = "1",
                UserName = "admin@soskd.uz",
                NormalizedUserName = "ADMIN@SOSKD.UZ",
                Email = "admin@soskd.uz",
                NormalizedEmail = "ADMIN@SOSKD.UZ",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            builder.Entity<IdentityUser>().HasData(adminUser);

            // Seed Roles
            var superAdminRole = new IdentityRole
            {
                Id = "1",
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN"
            };

            builder.Entity<IdentityRole>().HasData(superAdminRole);

            // Seed User Roles
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = "1",
                RoleId = "1"
            });

            // Seed News with proper UTC DateTimes
            builder.Entity<News>().HasData(
                new News
                {
                    Id = 1,
                    TitleUz = "SOS Bolalar shaharchasi yangi dasturni boshladi",
                    TitleRu = "SOS Детская деревня запустила новую программу",
                    TitleEn = "SOS Children's Village launches new program",
                    DescriptionUz = "<p>SOS Bolalar shaharchasi Toshkentda yangi ta'lim dasturini boshladi. Ushbu dastur bolalarning ijtimoiy va psixologik rivojlanishiga qaratilgan.</p>",
                    DescriptionRu = "<p>SOS Детская деревня в Ташкенте запустила новую образовательную программу. Эта программа направлена на социальное и психологическое развитие детей.</p>",
                    DescriptionEn = "<p>SOS Children's Village in Tashkent has launched a new educational program. This program focuses on the social and psychological development of children.</p>",
                    SlugUz = "yangi-dastur-boshlandi",
                    SlugRu = "novaya-programma-zapuschena",
                    SlugEn = "new-program-launched",
                    MetaTitleUz = "SOS Bolalar shaharchasi yangi dasturni boshladi",
                    MetaTitleRu = "SOS Детская деревня запустила новую программу",
                    MetaTitleEn = "SOS Children's Village launches new program",
                    MetaDescriptionUz = "SOS Bolalar shaharchasi Toshkentda yangi ta'lim dasturini boshladi",
                    MetaDescriptionRu = "SOS Детская деревня в Ташкенте запустила новую образовательную программу",
                    MetaDescriptionEn = "SOS Children's Village in Tashkent has launched a new educational program",
                    CategoryUz = "Ta'lim",
                    CategoryRu = "Образование",
                    CategoryEn = "Education",
                    Status = NewsStatus.Published,
                    SmallPhotoUrl = "/uploads/news/news1_small.jpg",
                    LargePhotoUrl = "/uploads/news/news1_large.jpg",
                    PublishedDate = baseUtcTime.AddDays(-5),
                    CreatedAt = baseUtcTime.AddDays(-5),
                    UpdatedAt = baseUtcTime.AddDays(-5)
                },
                new News
                {
                    Id = 2,
                    TitleUz = "Xalqaro bolalar kuni tadbiri",
                    TitleRu = "Мероприятие в честь Международного дня детей",
                    TitleEn = "International Children's Day Event",
                    DescriptionUz = "<p>1-iyun, Xalqaro bolalar kuni munosabati bilan SOS Bolalar shaharchasida katta bayram tadbiri o'tkazildi. Tadbirda 200 dan ortiq bola qatnashdi.</p>",
                    DescriptionRu = "<p>1 июня, в честь Международного дня детей, в SOS Детской деревне было проведено большое праздничное мероприятие. В мероприятии приняли участие более 200 детей.</p>",
                    DescriptionEn = "<p>On June 1st, International Children's Day, a large festive event was held at SOS Children's Village. More than 200 children participated in the event.</p>",
                    SlugUz = "bolalar-kuni-tadbiri",
                    SlugRu = "den-detej-meropriyatie",
                    SlugEn = "childrens-day-event",
                    MetaTitleUz = "Xalqaro bolalar kuni tadbiri",
                    MetaTitleRu = "Мероприятие в честь Международного дня детей",
                    MetaTitleEn = "International Children's Day Event",
                    MetaDescriptionUz = "1-iyun, Xalqaro bolalar kuni munosabati bilan SOS Bolalar shaharchasida katta bayram tadbiri o'tkazildi",
                    MetaDescriptionRu = "1 июня, в честь Международного дня детей, в SOS Детской деревне было проведено большое праздничное мероприятие",
                    MetaDescriptionEn = "On June 1st, International Children's Day, a large festive event was held at SOS Children's Village",
                    CategoryUz = "Tadbirlar",
                    CategoryRu = "События",
                    CategoryEn = "Events",
                    Status = NewsStatus.Published,
                    SmallPhotoUrl = "/uploads/news/news2_small.jpg",
                    LargePhotoUrl = "/uploads/news/news2_large.jpg",
                    PublishedDate = baseUtcTime.AddDays(-10),
                    CreatedAt = baseUtcTime.AddDays(-10),
                    UpdatedAt = baseUtcTime.AddDays(-10)
                }
            );

            // Seed Vacancies with proper UTC DateTimes
            builder.Entity<Vacancy>().HasData(
                new Vacancy
                {
                    Id = 1,
                    TitleUz = "Ijtimoiy ishchi",
                    TitleRu = "Социальный работник",
                    TitleEn = "Social Worker",
                    DescriptionUz = "<p>SOS Bolalar shaharchasi ijtimoiy ishchi lavozimiga xodim izlaydi. Nomzod bolalar bilan ishlash tajribasiga ega bo'lishi kerak.</p><p><strong>Talablar:</strong></p><ul><li>Ijtimoiy ish yoki psixologiya mutaxassisligi</li><li>Kamida 2 yil ish tajribasi</li><li>O'zbek va rus tillarini bilish</li></ul>",
                    DescriptionRu = "<p>SOS Детская деревня ищет сотрудника на должность социального работника. Кандидат должен иметь опыт работы с детьми.</p><p><strong>Требования:</strong></p><ul><li>Специальность социальная работа или психология</li><li>Минимум 2 года опыта работы</li><li>Знание узбекского и русского языков</li></ul>",
                    DescriptionEn = "<p>SOS Children's Village is looking for a Social Worker. The candidate should have experience working with children.</p><p><strong>Requirements:</strong></p><ul><li>Social work or psychology degree</li><li>Minimum 2 years of experience</li><li>Knowledge of Uzbek and Russian languages</li></ul>",
                    Status = VacancyStatus.Published,
                    PublishedDate = baseUtcTime.AddDays(-3),
                    Deadline = baseUtcTime.AddDays(14),
                    CreatedAt = baseUtcTime.AddDays(-3),
                    UpdatedAt = baseUtcTime.AddDays(-3)
                },
                new Vacancy
                {
                    Id = 2,
                    TitleUz = "Ta'lim koordinatori",
                    TitleRu = "Координатор по образованию",
                    TitleEn = "Education Coordinator",
                    DescriptionUz = "<p>Ta'lim dasturlarini rejalashtirish va amalga oshirish bo'yicha koordinator zarur.</p><p><strong>Mas'uliyatlar:</strong></p><ul><li>Ta'lim dasturlarini ishlab chiqish</li><li>O'qituvchilar bilan ishlash</li><li>Hisobotlar tayyorlash</li></ul>",
                    DescriptionRu = "<p>Требуется координатор по планированию и реализации образовательных программ.</p><p><strong>Обязанности:</strong></p><ul><li>Разработка образовательных программ</li><li>Работа с учителями</li><li>Подготовка отчетов</li></ul>",
                    DescriptionEn = "<p>An Education Coordinator is needed to plan and implement educational programs.</p><p><strong>Responsibilities:</strong></p><ul><li>Develop educational programs</li><li>Work with teachers</li><li>Prepare reports</li></ul>",
                    Status = VacancyStatus.Published,
                    PublishedDate = baseUtcTime.AddDays(-1),
                    Deadline = baseUtcTime.AddDays(21),
                    CreatedAt = baseUtcTime.AddDays(-1),
                    UpdatedAt = baseUtcTime.AddDays(-1)
                }
            );

            builder.Entity<MediaAboutUs>().HasData(
                new MediaAboutUs
                {
                    Id = 1,
                    TitleUz = "SOS Bolalar shaharchasi haqida yangi maqola",
                    TitleRu = "Новая статья о SOS Детской деревне",
                    TitleEn = "New article about SOS Children's Village",
                    DescriptionUz = "<p>Mahalliy matbuot SOS Bolalar shaharchasining faoliyati haqida batafsil maqola chop etdi. Maqolada tashkilotning bolalar hayotidagi ahamiyati ta'kidlangan.</p>",
                    DescriptionRu = "<p>Местная пресса опубликовала подробную статью о деятельности SOS Детской деревни. В статье подчеркивается важность организации в жизни детей.</p>",
                    DescriptionEn = "<p>Local press published a detailed article about the activities of SOS Children's Village. The article emphasizes the importance of the organization in children's lives.</p>",
                    PhotoUrl = "/uploads/media/media1.jpg",
                    SourceLink = "https://kun.uz/news/2024/11/15/sos-bolalar-shaharchasi",
                    Status = MediaStatus.Published,
                    PublishDate = baseUtcTime.AddDays(-7),
                    CreatedAt = baseUtcTime.AddDays(-7),
                    UpdatedAt = baseUtcTime.AddDays(-7)
                },
                new MediaAboutUs
                {
                    Id = 2,
                    TitleUz = "Televidenie SOS dasturlari haqida",
                    TitleRu = "Телевидение о программах SOS",
                    TitleEn = "Television coverage of SOS programs",
                    DescriptionUz = "<p>Milliy televidenie kanalida SOS Bolalar shaharchasining yangi dasturlari haqida maxsus reportaj efirga uzatildi.</p>",
                    DescriptionRu = "<p>На национальном телевидении был показан специальный репортаж о новых программах SOS Детской деревни.</p>",
                    DescriptionEn = "<p>A special report about new programs of SOS Children's Village was broadcasted on national television.</p>",
                    PhotoUrl = "/uploads/media/media2.jpg",
                    SourceLink = "https://uzreport.news/society/sos-bolalar-shaharchasi-yangi-dasturlar",
                    Status = MediaStatus.Published,
                    PublishDate = baseUtcTime.AddDays(-12),
                    CreatedAt = baseUtcTime.AddDays(-12),
                    UpdatedAt = baseUtcTime.AddDays(-12)
                }
            );

            //builder.Entity<Document>().HasData(
            //    new Document
            //    {
            //        Id = 1,
            //        TitleUz = "SOS Bolalar shaharchasi nizomi",
            //        TitleRu = "Устав SOS Детской деревни",
            //        TitleEn = "SOS Children's Village Charter",
            //        DescriptionUz = "Tashkilotning asosiy faoliyati va maqsadlarini belgilovchi hujjat",
            //        DescriptionRu = "Документ, определяющий основную деятельность и цели организации",
            //        DescriptionEn = "Document defining the main activities and goals of the organization",
            //        FileUrl = "/uploads/documents/sos_charter.pdf",
            //        FileName = "sos_charter.pdf",
            //        FileSizeBytes = 2048576, // 2MB
            //        Category = DocumentCategory.Documents,
            //        CategoryUz = "Hujjatlar",
            //        CategoryRu = "Документы",
            //        CategoryEn = "Documents",
            //        Status = DocumentStatus.Published,
            //        PublishDate = baseUtcTime.AddDays(-15),
            //        CreatedAt = baseUtcTime.AddDays(-15),
            //        UpdatedAt = baseUtcTime.AddDays(-15)
            //    },
            //    new Document
            //    {
            //        Id = 2,
            //        TitleUz = "Xavfsizlik siyosati",
            //        TitleRu = "Политика безопасности",
            //        TitleEn = "Security Policy",
            //        DescriptionUz = "Bolalar xavfsizligi va muhofazasi bo'yicha siyosat hujjati",
            //        DescriptionRu = "Документ политики по безопасности и защите детей",
            //        DescriptionEn = "Policy document on child safety and protection",
            //        FileUrl = "/uploads/documents/security_policy.pdf",
            //        FileName = "security_policy.pdf",
            //        FileSizeBytes = 1572864, // 1.5MB
            //        Category = DocumentCategory.SecurityAndValues,
            //        CategoryUz = "Xavfsizlik va qadriyatlar",
            //        CategoryRu = "Безопасность и ценности",
            //        CategoryEn = "Security and Values",
            //        Status = DocumentStatus.Published,
            //        PublishDate = baseUtcTime.AddDays(-8),
            //        CreatedAt = baseUtcTime.AddDays(-8),
            //        UpdatedAt = baseUtcTime.AddDays(-8)
            //    },
            //    new Document
            //    {
            //        Id = 3,
            //        TitleUz = "Yillik hisobot 2023",
            //        TitleRu = "Годовой отчет 2023",
            //        TitleEn = "Annual Report 2023",
            //        DescriptionUz = "SOS Bolalar shaharchasining 2023 yilgi faoliyat hisoboti",
            //        DescriptionRu = "Отчет о деятельности SOS Детской деревни за 2023 год",
            //        DescriptionEn = "SOS Children's Village activity report for 2023",
            //        FileUrl = "/uploads/documents/annual_report_2023.pdf",
            //        FileName = "annual_report_2023.pdf",
            //        FileSizeBytes = 5242880, // 5MB
            //        Category = DocumentCategory.Documents,
            //        CategoryUz = "Hujjatlar",
            //        CategoryRu = "Документы",
            //        CategoryEn = "Documents",
            //        Status = DocumentStatus.Published,
            //        PublishDate = baseUtcTime.AddDays(-30),
            //        CreatedAt = baseUtcTime.AddDays(-30),
            //        UpdatedAt = baseUtcTime.AddDays(-30)
            //    }
            //);

            builder.Entity<Sponsor>().HasData(
                new Sponsor
                {
                    Id = 1,
                    TitleUz = "Microsoft Corporation",
                    TitleRu = "Корпорация Майкрософт",
                    TitleEn = "Microsoft Corporation",
                    PhotoUrl = "/uploads/sponsors/microsoft_logo.png",
                    Link = "https://www.microsoft.com",
                    DisplayOrder = 1,
                    Status = SponsorStatus.Published,
                    CreatedAt = baseUtcTime.AddDays(-20),
                    UpdatedAt = baseUtcTime.AddDays(-20)
                },
                new Sponsor
                {
                    Id = 2,
                    TitleUz = "Google LLC",
                    TitleRu = "Гугл ЛЛК",
                    TitleEn = "Google LLC",
                    PhotoUrl = "/uploads/sponsors/google_logo.png",
                    Link = "https://www.google.com",
                    DisplayOrder = 2,
                    Status = SponsorStatus.Published,
                    CreatedAt = baseUtcTime.AddDays(-18),
                    UpdatedAt = baseUtcTime.AddDays(-18)
                },
                new Sponsor
                {
                    Id = 3,
                    TitleUz = "Apple Inc.",
                    TitleRu = "Эппл Инк.",
                    TitleEn = "Apple Inc.",
                    PhotoUrl = "/uploads/sponsors/apple_logo.png",
                    Link = "https://www.apple.com",
                    DisplayOrder = 3,
                    Status = SponsorStatus.Published,
                    CreatedAt = baseUtcTime.AddDays(-15),
                    UpdatedAt = baseUtcTime.AddDays(-15)
                },
                new Sponsor
                {
                    Id = 4,
                    TitleUz = "Meta Platforms",
                    TitleRu = "Мета Платформс",
                    TitleEn = "Meta Platforms",
                    PhotoUrl = "/uploads/sponsors/meta_logo.png",
                    Link = "https://www.meta.com",
                    DisplayOrder = 4,
                    Status = SponsorStatus.Unpublished, // Example of unpublished sponsor
                    CreatedAt = baseUtcTime.AddDays(-10),
                    UpdatedAt = baseUtcTime.AddDays(-10)
                }
            );

            builder.Entity<ContactApplication>().HasData(
    new ContactApplication
    {
        Id = 1,
        FullName = "John Doe",
        Email = "john.doe@example.com",
        Phone = "+998901234567",
        City = "Tashkent",
        Message = "I would like to learn more about your programs and how I can help support the children.",
        CreatedAt = baseUtcTime.AddDays(-5),
        IpAddress = "192.168.1.100"
    },
    new ContactApplication
    {
        Id = 2,
        FullName = "Maria Petrova",
        Email = "maria.petrova@example.com",
        Phone = "+998907654321",
        City = "Samarkand",
        Message = "Hello, I am interested in volunteering opportunities. Could you please provide more information about how to get involved?",
        CreatedAt = baseUtcTime.AddDays(-2),
        IpAddress = "192.168.1.101"
    },
    new ContactApplication
    {
        Id = 3,
        FullName = "Ahmad Karimov",
        Email = "ahmad.karimov@example.com",
        City = "Bukhara",
        Message = "Salom! Men SOS Bolalar shaharchasi haqida ko'proq ma'lumot olishni xohlayman. Qanday yordam bera olaman?",
        CreatedAt = baseUtcTime.AddDays(-1),
        IpAddress = "192.168.1.102"
    }
);

        }
    }
}