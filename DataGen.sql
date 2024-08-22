DECLARE @ClinicCount INT, @DiseaseCount INT, @RoomCount INT;
SET @ClinicCount = 1;
SET @DiseaseCount = 1;
SET @RoomCount = 1;

WHILE (@ClinicCount < 9)
BEGIN
	--SELECT IDENT_CURRENT('Clinics')

	INSERT INTO Clinics([Name])
		VALUES ( CONCAT(N'Clinic-', CAST(@ClinicCount AS NVARCHAR)));
		
	WHILE(@DiseaseCount < 10)
	BEGIN 
		INSERT INTO Diseases(ClinicId, [Name])
			VALUES (IDENT_CURRENT('Clinics'), CONCAT(N'Disease-', CAST(@ClinicCount AS NVARCHAR), N'-', CAST(@DiseaseCount AS NVARCHAR)));
		SET @DiseaseCount = @DiseaseCount + 1;
	END;

	WHILE(@RoomCount < 20)
	BEGIN 
		INSERT INTO Rooms(ClinicId, [RoomNumber])
			VALUES ( IDENT_CURRENT('Clinics'), CONCAT(N'Room-',CAST(@ClinicCount AS NVARCHAR), N'-', CAST(@RoomCount AS NVARCHAR)));
			IF(@RoomCount = 1 OR @RoomCount = 2)
			BEGIN
				INSERT INTO Beds(RoomId, BedInfo)
					VALUES (IDENT_CURRENT('Rooms'), N'Middle');
			END;
			ELSE IF(@RoomCount < 10)
			BEGIN
				INSERT INTO Beds(RoomId,BedInfo)
					VALUES (IDENT_CURRENT('Rooms'), N'Window'),
						(IDENT_CURRENT('Rooms'),  N'Corridor');
			END;
			ELSE
			BEGIN
				INSERT INTO Beds(RoomId, BedInfo)
					VALUES (IDENT_CURRENT('Rooms'), N'Window'),
						(IDENT_CURRENT('Rooms'), N'Middle'),
						(IDENT_CURRENT('Rooms'), N'Corridor');
			END;
		SET @RoomCount = @RoomCount + 1;
	END;
	
	SET @RoomCount = 1;
	SET @DiseaseCount = 1;
	SET @ClinicCount = @ClinicCount + 1;

END;