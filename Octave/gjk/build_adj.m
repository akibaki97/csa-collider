function Adj = build_adj(V,F)

    global DEBUG;
    if exist('DEBUG'),
        DEBUG__ = DEBUG;
    else
        DEBUG__ = false;
    endif

    % build adjacencies into A matrix
    n = rows(V);
    A = zeros(n);
    m = rows(F);
    for k = 1:m,
        A(F(k,1),F(k,2)) = 1;
        A(F(k,1),F(k,3)) = 1;
        A(F(k,2),F(k,1)) = 1;
        A(F(k,2),F(k,3)) = 1;
        A(F(k,3),F(k,1)) = 1;
        A(F(k,3),F(k,2)) = 1;
    endfor

    % create edge-list representation
    for i = 1:n
        Adj(i) = lnode();
    endfor

    for i = 1:n,
        for j = 1:n,
            if A(i,j) == 1,
                ln = Adj(i).next;
                Adj(i).next = lnode(j);
                Adj(i).next.next = ln;
            endif
        endfor
    endfor

    if DEBUG__,
        printf("\n");
        for i = 1:n,
            ln = Adj(i).next;
            printf("%d:  ",i);
            while !isempty(ln),
                printf("%d  ",ln.data);
                ln = ln.next;
            endwhile
            printf("\n");
        endfor
        printf("\n");
    endif

endfunction