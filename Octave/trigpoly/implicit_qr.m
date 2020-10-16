% Implicit QR algorithm designed for finding roots of trigonometric polynomials
% Parameters:
%   A: obviously the companion matrix
%   bb: tells if the function should use inversion shifts
function lambda = implicit_qr(A,bb)

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

    lambda = zeros(n,1);

    for i = 1:40,
        l = n;
        % Test if A is properly Hessenberg
        for j = n-1:-1:1,
            if abs(A(j+1,j)) < eps,
                lambda(j+1:l) = implicit_qr(A(j+1:l,j+1:l),bb);
                l = j;
            endif
        endfor

        if l < n,
            if l > 2,
                % Get new shifts from previous ones by inversion
                S = get_shifts(lambda(l+1:n));
                m = length(S);

                if m == l,
                    lambda(1:l) = S
                    return
                endif

                if bb && m > 1,
                    % Shift with inversions, then chase the bulge
                    A = bulge_chase(A(1:l,1:l),m,S);
                    num_it++;
                endif
            endif
            lambda(1:l) = implicit_qr(A(1:l,1:l),bb);
            return
        endif

        shifts = eig(A(n-1:n,n-1:n));

        A = bulge_chase(A,2,shifts);
        num_it++;
    endfor
endfunction

function S = get_shifts(d)

    k = 1; S = [];
    m = length(d);
    eps = 1e-6;
    for i = 1:m
        abs_di = abs(d(i));
        if abs_di > eps && abs(abs_di-1) > eps,
            x = real(d(i))/(abs_di^2);
            y = imag(d(i))/(abs_di^2);
            b = false;
            for j = i+1:m,
                if abs(real(d(j))-x) < eps && abs(imag(d(j)-y)) < eps,
                    d(j) = 0;
                    b = true;
                endif
            endfor

            if !b,
                S(k++) = x + I*y;
            endif
        endif
    endfor

endfunction
