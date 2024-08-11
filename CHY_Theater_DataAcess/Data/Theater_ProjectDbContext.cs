using System;
using System.Collections.Generic;
using CHY_Theater_Models;
using CHY_Theater_Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CHY_Theater_DataAcess.Data;

public partial class Theater_ProjectDbContext : IdentityDbContext<IdentityUser>
{
    public Theater_ProjectDbContext()
    {
    }

    public Theater_ProjectDbContext(DbContextOptions<Theater_ProjectDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<RewardPoint> RewardPoints { get; set; }

    public DbSet<EcpayOrder> EcpayOrders { get; set; }

    public virtual DbSet<CarouselItem> CarouselItems { get; set; }

    public virtual DbSet<News> News { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<Test> Test { get; set; }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Auditorium> Auditoriums { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingCoupon> BookingCoupons { get; set; }

    public virtual DbSet<BookingSeatsDetail> BookingSeatsDetails { get; set; }

    public virtual DbSet<BookingSnack> BookingSnacks { get; set; }

    public virtual DbSet<BookingTicketTypesDetail> BookingTicketTypesDetails { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Habbit> Habbits { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<MovieActor> MovieActors { get; set; }

    public virtual DbSet<MovieClass> MovieClasses { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<ShowSeat> ShowSeats { get; set; }

    public virtual DbSet<Snack> Snacks { get; set; }

    public virtual DbSet<Theater> Theaters { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<Twin> Twins { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserHabbit> UserHabbits { get; set; }

    public virtual DbSet<UserPassword> UserPasswords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-UQOH0QC\\SQLEXPRESS01;Database= Theater_Project;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EcpayOrder>()
           .HasOne(e => e.Booking)
           .WithMany()
           .HasForeignKey(e => e.BookingId);

        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.ActorId).HasName("PK__Actors__57B3EA4B0E626426");

            entity.Property(e => e.ActorName).HasMaxLength(50);
        });

        modelBuilder.Entity<Auditorium>(entity =>
        {
            entity.HasKey(e => e.AuditoriumId).HasName("PK__Auditori__6E91B185D6E4C686");

            entity.Property(e => e.AuditoriumName).HasMaxLength(50);
            entity.Property(e => e.AuditoriumType).HasMaxLength(50);
            entity.Property(e => e.TheaterId).HasColumnName("Theater_Id");

            entity.HasOne(d => d.Theater).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.TheaterId)
                .HasConstraintName("FK__Auditoriu__Theat__5DCAEF64");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED2C97DB6B");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.MerchantTradeNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Showing).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowingId)
                .HasConstraintName("FK__Bookings__Showin__797309D9");
        });

        modelBuilder.Entity<BookingCoupon>(entity =>
        {
            entity.HasKey(e => e.BookingCouponId).HasName("PK__BookingC__7A33E467D849637D");

            entity.Property(e => e.BookingCouponId).HasColumnName("BookingCouponID");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.CouponId).HasColumnName("CouponID");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCoupons)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingCo__Booki__08B54D69");

            entity.HasOne(d => d.Coupon).WithMany(p => p.BookingCoupons)
                .HasForeignKey(d => d.CouponId)
                .HasConstraintName("FK__BookingCo__Coupo__09A971A2");
        });

        modelBuilder.Entity<BookingSeatsDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__BookingS__135C316D59B40F00");

            entity.ToTable("BookingSeats_Detail");

            entity.Property(e => e.BookingId).HasColumnName("Booking_Id");
            entity.Property(e => e.ShowSeatId).HasColumnName("ShowSeat_Id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingSeatsDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingSe__Booki__01142BA1");

            entity.HasOne(d => d.ShowSeat).WithMany(p => p.BookingSeatsDetails)
                .HasForeignKey(d => d.ShowSeatId)
                .HasConstraintName("FK__BookingSe__ShowS__02084FDA");
        });

        modelBuilder.Entity<BookingSnack>(entity =>
        {
            entity.HasKey(e => e.BookingSnackId).HasName("PK__BookingS__BE71E92CE04B6DF6");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingSnacks)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingSn__Booki__04E4BC85");

            entity.HasOne(d => d.Snack).WithMany(p => p.BookingSnacks)
                .HasForeignKey(d => d.SnackId)
                .HasConstraintName("FK__BookingSn__Snack__05D8E0BE");
        });

        modelBuilder.Entity<BookingTicketTypesDetail>(entity =>
        {
            entity.HasKey(e => e.TicketTypesDetailId).HasName("PK__BookingT__1CDBC808754071BE");

            entity.ToTable("BookingTicketTypes_Detail");

            entity.Property(e => e.BookingId).HasColumnName("Booking_Id");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingTicketTypesDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingTi__Booki__7D439ABD");

            entity.HasOne(d => d.TicketType).WithMany(p => p.BookingTicketTypesDetails)
                .HasForeignKey(d => d.TicketTypeId)
                .HasConstraintName("FK__BookingTi__Ticke__7E37BEF6");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Classes__CB1927C01BC8ED5A");

            entity.Property(e => e.ClassName).HasMaxLength(50);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentsId).HasName("PK__Comments__9487C7CC0E8FED1E");

            entity.Property(e => e.CommentMessage).HasMaxLength(200);
            entity.Property(e => e.CommentTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MovieId).HasColumnName("Movie_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.Movie).WithMany(p => p.Comments)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__Movie___5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__User_I__5535A963");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C66259BADB5D91E");

            entity.Property(e => e.ContactDescription).HasMaxLength(300);
            entity.Property(e => e.ContactStatus).HasMaxLength(10);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IssueType).HasMaxLength(10);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contacts__User_I__10566F31");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PK__Coupons__384AF1DAC55A378D");

            entity.HasIndex(e => e.CouponCode, "UQ__Coupons__D349080072329089").IsUnique();

            entity.Property(e => e.CouponId).HasColumnName("CouponID");
            entity.Property(e => e.CouponCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CurrentUsageCount).HasDefaultValue(0);
            entity.Property(e => e.DiscountType).HasMaxLength(20);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Habbit>(entity =>
        {
            entity.HasKey(e => e.HabbitId).HasName("PK__Habbits__37878EF4057BE2B2");

            entity.Property(e => e.Habbit1)
                .HasMaxLength(10)
                .HasColumnName("Habbit");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2941A0C4DBD58");

            entity.Property(e => e.DirectorName).HasMaxLength(50);
            entity.Property(e => e.Language).HasMaxLength(10);
            entity.Property(e => e.MovieEnglishName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MovieName).HasMaxLength(50);
            entity.Property(e => e.MovieState)
                                      .HasDefaultValue(null); 
            entity.Property(e => e.Movievideo).IsUnicode(false);
        });

        modelBuilder.Entity<MovieActor>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.ActorId }).HasName("PK__Movie_Ac__B4DF441D60805C80");

            entity.ToTable("Movie_Actor");

            entity.Property(e => e.MovieId).HasColumnName("Movie_Id");
            entity.Property(e => e.ActorId).HasColumnName("Actor_Id");

            entity.HasOne(d => d.Actor).WithMany(p => p.MovieActors)
                .HasForeignKey(d => d.ActorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movie_Act__Actor__4316F928");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieActors)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movie_Act__Movie__4222D4EF");
        });

        modelBuilder.Entity<MovieClass>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.ClassId }).HasName("PK__Movie_Cl__11817476F019CDDE");

            entity.ToTable("Movie_Class");

            entity.Property(e => e.MovieId).HasColumnName("Movie_Id");
            entity.Property(e => e.ClassId).HasColumnName("Class_Id");

            entity.HasOne(d => d.Class).WithMany(p => p.MovieClasses)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movie_Cla__Class__3D5E1FD2");

            entity.HasOne(d => d.Movie).WithMany(p => p.MovieClasses)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movie_Cla__Movie__3C69FB99");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PaymentT__55433A6BB7EEDB04");

            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Booking).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__PaymentTr__Booki__0C85DE4D");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => new { e.MovieId, e.UserId }).HasName("PK__Rating__688EDD32FBA3EE38");

            entity.ToTable("Rating");

            entity.Property(e => e.MovieId).HasColumnName("Movie_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.Rating1).HasColumnName("Rating");

            entity.HasOne(d => d.Movie).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__Movie_Id__5812160E");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__User_Id__59063A47");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seats__311713F380106F28");

            entity.Property(e => e.AuditoriumId).HasColumnName("Auditorium_Id");
            entity.Property(e => e.SeatRow).HasMaxLength(10);
            entity.Property(e => e.SeatStatus).HasMaxLength(20);
            entity.Property(e => e.SeatType).HasMaxLength(20);

            entity.HasOne(d => d.Auditorium).WithMany(p => p.Seats)
                .HasForeignKey(d => d.AuditoriumId)
                .HasConstraintName("FK__Seats__Auditoriu__60A75C0F");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.ShowId).HasName("PK__Shows__6DE3E0B2D6FADC45");

            entity.Property(e => e.AuditoriumId).HasColumnName("Auditorium_Id");
            entity.Property(e => e.MovieId).HasColumnName("Movie_Id");
            entity.Property(e => e.ShowDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.Auditorium).WithMany(p => p.Shows)
                .HasForeignKey(d => d.AuditoriumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shows__Auditoriu__6477ECF3");

            entity.HasOne(d => d.Movie).WithMany(p => p.Shows)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shows__Movie_Id__6383C8BA");
        });

        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.HasKey(e => e.ShowSeatId).HasName("PK__ShowSeat__9536FC93DE684D92");

            entity.Property(e => e.SeatId).HasColumnName("Seat_Id");
            entity.Property(e => e.ShowId).HasColumnName("Show_Id");
            entity.Property(e => e.ShowSeatStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Seat).WithMany(p => p.ShowSeats)
                .HasForeignKey(d => d.SeatId)
                .HasConstraintName("FK__ShowSeats__Seat___68487DD7");

            entity.HasOne(d => d.Show).WithMany(p => p.ShowSeats)
                .HasForeignKey(d => d.ShowId)
                .HasConstraintName("FK__ShowSeats__Show___6754599E");
        });

        modelBuilder.Entity<Snack>(entity =>
        {
            entity.HasKey(e => e.SnackId).HasName("PK__Snacks__320A85CBE113394B");

            entity.Property(e => e.SnackName).HasMaxLength(50);
            entity.Property(e => e.SnackSize).HasMaxLength(50);
        });

        modelBuilder.Entity<Theater>(entity =>
        {
            entity.HasKey(e => e.TheaterId).HasName("PK__Theaters__4D68B2191C691A37");

            entity.Property(e => e.TheaterDescription).HasMaxLength(200);
            entity.Property(e => e.TheaterEmail).HasMaxLength(50);
            entity.Property(e => e.TheaterLocation).HasMaxLength(50);
            entity.Property(e => e.TheaterName).HasMaxLength(20);
            entity.Property(e => e.TheaterPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeId).HasName("PK__TicketTy__6CD68431CF23329C");

            entity.Property(e => e.TicketDescription).HasMaxLength(200);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Twin>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.InviteeId, e.ShowId }).HasName("PK__Twins__F63A532D71BFCBA4");

            entity.Property(e => e.UserId).HasColumnName("User_Id");
            entity.Property(e => e.InviteeId).HasColumnName("Invitee_Id");
            entity.Property(e => e.ShowId).HasColumnName("Show_Id");

            entity.HasOne(d => d.Invitee).WithMany(p => p.TwinInvitees)
                .HasForeignKey(d => d.InviteeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Twins__Invitee_I__6EF57B66");

            entity.HasOne(d => d.Show).WithMany(p => p.Twins)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Twins__Show_Id__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.TwinUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Twins__User_Id__6E01572D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDD41E9DF");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053424887393").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmailConfirm).HasDefaultValue(false);
            entity.Property(e => e.Mbti)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("MBTI");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Sex).HasMaxLength(10);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserHabbit>(entity =>
        {
            entity.HasKey(e => new { e.HabbitId, e.UserId }).HasName("PK__User_Hab__0DE6E2D85C36AF4C");

            entity.ToTable("User_Habbit");

            entity.Property(e => e.HabbitId).HasColumnName("Habbit_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.Habbit).WithMany(p => p.UserHabbits)
                .HasForeignKey(d => d.HabbitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Habb__Habbi__4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.UserHabbits)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Habb__User___4CA06362");
        });

        modelBuilder.Entity<UserPassword>(entity =>
        {
            entity.HasKey(e => e.UserPasswordId).HasName("PK__UserPass__FD2F7D5F56B60D05");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserPasswords)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserPassw__UserI__4F7CD00D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
