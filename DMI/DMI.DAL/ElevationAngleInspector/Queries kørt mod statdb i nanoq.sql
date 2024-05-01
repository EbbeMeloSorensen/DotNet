--24-04-2024
UPDATE leeindex SET start_time = '2024-04-23 10:00:00' WHERE statid = 560020 AND start_time::TEXT LIKE ('2024-04-23%');
UPDATE leeindex SET start_time = '2024-02-01 11:00:00' WHERE statid = 562950 AND start_time::TEXT LIKE ('2024-02-01%');
UPDATE leeindex SET start_time = '2024-02-01 11:00:00' WHERE statid = 563150 AND start_time::TEXT LIKE ('2024-02-01%');
UPDATE leeindex SET end_time = '2024-04-23 10:00:00' WHERE statid = 560020 AND start_time::TEXT LIKE ('2022-06-30%');

UPDATE leeindex SET end_time = '2024-04-23 10:00:00' WHERE statid = 564220 AND start_time = '2022-06-30 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (564220, '2024-04-23 10:00:00', 'infinity', 11, 15, 9, 15, 12, 8, 35, 14, 14);

--25-04-2024
UPDATE leeindex SET end_time = '2024-04-24 10:00:00' WHERE statid = 566520 AND start_time = '2022-06-30 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (566520, '2024-04-24 10:00:00', 'infinity', 21, 12, 21, 35, 30, 20, 30, 25, 22);

UPDATE leeindex SET end_time = '2024-04-24 10:00:00' WHERE statid = 573220 AND start_time = '2022-06-29 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (573220, '2024-04-24 10:00:00', 'infinity', 7, 9, 5, 12, 14, 19, 9, 11, 9);

UPDATE leeindex SET end_time = '2024-04-24 10:00:00' WHERE statid = 573320 AND start_time = '2022-06-29 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (573320, '2024-04-24 10:00:00', 'infinity', 17, 14, 8, 28, 32, 27, 14, 14, 16);

UPDATE leeindex SET end_time = '2024-04-09 10:00:00' WHERE statid = 603100 AND start_time = '2022-05-05 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (603100, '2024-04-09 10:00:00', 'infinity', 2, 7, 8, 12, 15, 24, 6, 1, 7);

UPDATE leeindex SET end_time = '2024-04-08 10:00:00' WHERE statid = 606500 AND start_time = '2022-05-06 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (606500, '2024-04-08 10:00:00', 'infinity', 1, 1, 3, 4, 3, 2, 1, 1, 2);

UPDATE leeindex SET end_time = '2024-04-03 10:00:00' WHERE statid = 565520 AND start_time = '2022-04-04 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (565520, '2024-04-03 10:00:00', 'infinity', 12, 12, 13, 16, 12, 11, 44, 37, 19);

UPDATE leeindex SET end_time = '2024-04-03 10:00:00' WHERE statid = 572520 AND start_time = '2022-04-04 10:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (572520, '2024-04-03 10:00:00', 'infinity', 22, 30, 23, 10, 11, 15, 28, 26, 23);

UPDATE leeindex SET end_time = '2023-09-19 10:00:00' WHERE statid = 608800 AND start_time = '2015-10-06 00:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (608800, '2023-09-19 10:00:00', 'infinity', 7, 7, 4, 10, 9, 6, 4, 4, 6);

UPDATE leeindex SET end_time = '2023-08-02 10:00:00' WHERE statid = 525520 AND start_time = '2017-06-26 00:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (525520, '2023-08-02 10:00:00', 'infinity', 10, 15, 23, 36, 20, 15, 9, 10, 16);

UPDATE leeindex SET end_time = '2023-07-11 10:00:00' WHERE statid = 569720 AND start_time = '2015-11-24 00:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (569720, '2023-07-11 10:00:00', 'infinity', 16, 20, 28, 14, 11, 7, 3, 5, 15);

INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (527820, '2023-06-28 10:00:00', 'infinity', 1, 1, 1, 4, 1, 2, 2, 3, 2);

UPDATE leeindex SET start_time = '2023-02-23 11:00:00' WHERE statid = 598120 AND start_time = '2023-02-23 00:00:00';

UPDATE leeindex SET end_time = '2023-01-30 11:00:00' WHERE statid = 516920 AND start_time = '2015-10-07 00:00:00';
INSERT INTO public.leeindex (statid, start_time, end_time, s, sw, w, nw, n, ne, e, se, index) VALUES (516920, '2023-01-30 11:00:00', 'infinity', 2, 1, 4, 7, 6, 7, 5, 2, 3);

