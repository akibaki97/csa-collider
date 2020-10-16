function lambda = qr_alg(A,t)

    global num_it;

    if rows(A) == 1,
        lambda = A;
        return;
    elseif rows(A) == 2,
        lambda = eig(A);
        return;
    endif

    n = rows(A);
    eps = 1e-8;

    shifts = eig(A(n-1:n,n-1:n)); 

    for i = 1:40,
        
        num_it += n;

        % explicit qr
        pA = (A - shifts(1)*eye(n)) * (A - shifts(2)*eye(n));
        [Q,R] = qr(pA);
        A = Q' * A * Q;

        shifts = eig(A(n-1:n,n-1:n));

        if abs(A(n-1,n-2)) < eps,
            lambda(1:2) = eig(A(n-1:n,n-1:n));

            if t && abs(abs(lambda(1)) - 1) > 1e-4 && abs(abs(lambda(2)) - 1) > 1e-4,
                z1 = 1 / conj(lambda(1));
                z2 = 1 / conj(lambda(2));
                pA = (A(1:n-2,1:n-2) - z1 * eye(n-2)) * (A(1:n-2,1:n-2) - z2 * eye(n-2));
                [Q,R] = qr(pA);
                A(1:n-2,1:n-2) = Q' * A(1:n-2,1:n-2) * Q;
                num_it++;
            endif

            l = 1;
            for j = 1:n-3,
                if abs(A(j+1,j)) < eps,
                    lambda(l+2:j+2) = qr_alg(A(l:j,l:j),t);
                    l = j+1;
                endif
            endfor

            lambda(l+2:n) = qr_alg(A(l:n-2,l:n-2),t);
            return;
        endif
    endfor

    lambda = diag(A);
endfunction