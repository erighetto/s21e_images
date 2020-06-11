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

    SET @name  = (SELECT 
            attribute_id
        FROM
            eav_attribute
        WHERE
            attribute_code = 'name' and frontend_label = 'Product Name');
    SELECT JSON_OBJECT('CodArt', TRIM(catalog_product_entity.sku), 'DescArticolo', ANY_VALUE(v.value), 'CodEan', ANY_VALUE(pec.code))
    FROM catalog_product_entity
    LEFT JOIN product_ean_codes pec ON catalog_product_entity.entity_id = pec.product_id
    LEFT JOIN catalog_product_entity_varchar v ON catalog_product_entity.entity_id = v.entity_id AND v.attribute_id = @name
    LEFT JOIN catalog_product_entity_media_gallery_value AS gv ON catalog_product_entity.entity_id = gv.entity_id 
    LEFT JOIN catalog_product_entity_media_gallery AS g ON gv.value_id = gv.value_id
    WHERE g.value IS NULL
    GROUP BY TRIM(catalog_product_entity.sku);