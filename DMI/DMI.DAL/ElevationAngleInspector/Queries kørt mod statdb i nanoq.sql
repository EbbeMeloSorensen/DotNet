UPDATE leeindex SET start_time = '2024-04-23 10:00:00' WHERE statid = 560020 AND start_time::TEXT LIKE ('2024-04-23%');
UPDATE leeindex SET start_time = '2024-02-01 11:00:00' WHERE statid = 562950 AND start_time::TEXT LIKE ('2024-02-01%');
UPDATE leeindex SET start_time = '2024-02-01 11:00:00' WHERE statid = 563150 AND start_time::TEXT LIKE ('2024-02-01%');
UPDATE leeindex SET end_time = '2024-04-23 10:00:00' WHERE statid = 560020 AND start_time::TEXT LIKE ('2022-06-30%');

UPDATE leeindex SET end_time = '2024-04-23 10:00:00' WHERE statid = 564220 AND start_time = '2022-06-30 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (564220, '2024-04-23 10:00:00', 'infinity', 11, 15, 9, 15, 12, 8, 35, 14, 14);
