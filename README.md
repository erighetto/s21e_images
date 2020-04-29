# s21e images

## images collect

    SELECT TRIM(p.CodArt) as CodArt, ANY_VALUE(e.CodEAN) AS CodEan, ANY_VALUE(p.DescArticolo) AS DescArticolo 
    FROM tblarticolo p 
    JOIN tblean e USING (CodArt) 
    GROUP BY TRIM(p.CodArt) 
    ORDER BY TRIM(p.CodArt);

## images export