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

    DROP TEMPORARY TABLE IF EXISTS supergabry_symfony.tbl_temp1;
    DROP TEMPORARY TABLE IF EXISTS supergabry_symfony.tbl_temp2;

    CREATE TEMPORARY TABLE supergabry_symfony.tbl_temp1 SELECT a.sku FROM supergabry_magento.catalog_product_entity AS a 
    LEFT JOIN supergabry_magento.catalog_product_entity_media_gallery_value AS b ON a.entity_id = b.entity_id 
    LEFT JOIN supergabry_magento.catalog_product_entity_media_gallery AS c ON b.value_id = c.value_id WHERE c.value IS NULL;

    CREATE TEMPORARY TABLE supergabry_symfony.tbl_temp2 SELECT a.sku FROM supergabry_magento.catalog_product_entity AS a 
    LEFT JOIN supergabry_magento.catalog_product_entity_media_gallery_value_to_entity AS b ON a.entity_id = b.entity_id 
    LEFT JOIN supergabry_magento.catalog_product_entity_media_gallery AS c ON b.value_id = c.value_id WHERE c.value IS NULL;

    SELECT TRIM(p.CodArt) as CodArt, ANY_VALUE(e.CodEAN) AS CodEan, ANY_VALUE(p.DescArticolo) AS DescArticolo 
    FROM supergabry_symfony.tblarticolo p 
    JOIN supergabry_symfony.tblean e USING (CodArt) 
    WHERE TRIM(p.CodArt) IN (SELECT trim(t1.sku) FROM supergabry_symfony.tbl_temp1 AS t1
        INNER JOIN supergabry_symfony.tbl_temp2 AS t2 ON t1.sku = t2.sku)
    GROUP BY TRIM(p.CodArt) 
    ORDER BY TRIM(p.CodArt);