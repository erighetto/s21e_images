# s21e images

## images collect

    SELECT TRIM(p.CodArt) as CodArt, ANY_VALUE(e.CodEAN) AS CodEan, ANY_VALUE(p.DescArticolo) AS DescArticolo 
    FROM tblarticolo p 
    JOIN tblean e USING (CodArt) 
    GROUP BY TRIM(p.CodArt) 
    ORDER BY TRIM(p.CodArt);

## images export

Generate the xml for importing products


## images refine

Kind of web scraper

    SELECT TRIM(a.sku) FROM catalog_product_entity AS a 
    LEFT JOIN catalog_product_entity_media_gallery_value AS b ON a.entity_id = b.entity_id 
    LEFT JOIN catalog_product_entity_media_gallery AS c ON b.value_id = c.value_id WHERE c.value IS NULL;