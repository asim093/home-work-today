using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace bus_reservation.Models;

public partial class BusReservationSystemContext : DbContext
{
    public BusReservationSystemContext()
    {
    }

    public BusReservationSystemContext(DbContextOptions<BusReservationSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Bus> Buses { get; set; }

    public virtual DbSet<BusType> BusTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=.;initial catalog=Bus_Reservation;user id=sa;password=aptech; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Admin__3214EC27A2F03A97");

            entity.ToTable("Admin");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__tmp_ms_x__35ABFDC0DE992DF8");

            entity.Property(e => e.BookingId).HasColumnName("Booking_Id");
            entity.Property(e => e.AdminId).HasColumnName("Admin_Id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Booking_Date");
            entity.Property(e => e.BranchId).HasColumnName("Branch_Id");
            entity.Property(e => e.BusId).HasColumnName("Bus_Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.CustomerAge).HasColumnName("Customer_Age");
            entity.Property(e => e.CustomerContact)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("Customer_Contact");
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Customer_Email");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Customer_Name");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");
            entity.Property(e => e.SeatNumber).HasColumnName("Seat_Number");
            entity.Property(e => e.StatusBooking)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValueSql("('Confirmed')")
                .HasColumnName("Status_Booking");

            entity.HasOne(d => d.Admin).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_Ticket_Booking_Admin");

            entity.HasOne(d => d.Branch).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Ticket_Booking_Branch");

            entity.HasOne(d => d.Bus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Booking_Bus");

            entity.HasOne(d => d.Employee).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Ticket_Booking_Employee");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Branch__3213E83F38638704");

            entity.ToTable("Branch");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("branch_code");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contact");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__tmp_ms_x__B0524D196201E5FE");

            entity.ToTable("Bus");

            entity.Property(e => e.BusId).HasColumnName("Bus_Id");
            entity.Property(e => e.ArrivalTime)
                .HasColumnType("datetime")
                .HasColumnName("Arrival_Time");
            entity.Property(e => e.AvailableSeats).HasColumnName("Available_Seats");
            entity.Property(e => e.BusImage)
                .IsUnicode(false)
                .HasColumnName("Bus_Image");
            entity.Property(e => e.BusNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("Bus_Number");
            entity.Property(e => e.BusTypeId).HasColumnName("Bus_Type_Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.DepartureTime)
                .HasColumnType("datetime")
                .HasColumnName("Departure_Time");
            entity.Property(e => e.RouteId).HasColumnName("Route_Id");
            entity.Property(e => e.TotalSeats).HasColumnName("Total_Seats");

            entity.HasOne(d => d.BusType).WithMany(p => p.Buses)
                .HasForeignKey(d => d.BusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bus_ToBusType");

            entity.HasOne(d => d.Route).WithMany(p => p.Buses)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bus_ToRoute");
        });

        modelBuilder.Entity<BusType>(entity =>
        {
            entity.HasKey(e => e.BusTypeId).HasName("PK__Bus_Type__A2B1F2DBD81E60EA");

            entity.ToTable("Bus_Type");

            entity.Property(e => e.BusTypeId).HasColumnName("Bus_Type_Id");
            entity.Property(e => e.BusTypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Bus_Type_Name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07AC82C7C0");

            entity.ToTable("Employee");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.BranchId).HasColumnName("Branch_Id");
            entity.Property(e => e.Contact)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("date_created");
            entity.Property(e => e.Designation)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("designation");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeImage)
                .IsUnicode(false)
                .HasColumnName("employee_image");
            entity.Property(e => e.Fathername)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MartialStatus)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("martial_status");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Salary).HasColumnName("salary");

            entity.HasOne(d => d.Branch).WithMany(p => p.Employees)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Branch");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__D2BA00E2A64D4195");

            entity.ToTable("Location");

            entity.Property(e => e.LocationId).HasColumnName("Location_Id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.LocationName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Location_Name");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__tmp_ms_x__80979B4D72A06C49");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Distance).HasColumnName("distance");
            entity.Property(e => e.RouteName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.DestinationPlaceNavigation).WithMany(p => p.RouteDestinationPlaceNavigations)
                .HasForeignKey(d => d.DestinationPlace)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Routes_ToDestinationPlace");

            entity.HasOne(d => d.StartingPlaceNavigation).WithMany(p => p.RouteStartingPlaceNavigations)
                .HasForeignKey(d => d.StartingPlace)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Routes_ToStartingPlace");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
