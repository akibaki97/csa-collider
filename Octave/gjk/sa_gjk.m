function v = sa_gjk(A,B)

    global DEBUG;
    DEBUG__ = false;

    if DEBUG__, 
        figure(1);
        axis equal;
        plot_polygon(A);
        plot_polygon(B);

        figure(2);
        axis equal;
        CSO = cso(A,B);
        plot_polygon(CSO);
        waitforbuttonpress;
    endif

    % space dimension
    n = columns(A);

    % starting point: can be any point, not necessarily in A-B
    v = A(1,:) - B(1,:)

    % utils
    W = AW = BW = zeros(n+1,n)                  % vec array holds the simplex vertices
    y = 0;                                      % represents the current simplex as bitset
    global max_vertex = n+1;                    % maximum number of vertices in the simplex
    delta = zeros(2^max_vertex-1, max_vertex);  % matrix, holds the determinants for Johnson's distance alg.
    d = zeros(max_vertex,max_vertex);           % matrix, holds the differences of simplex vertices (for J's alg.)
    epsTol = eps * 1e10;                        % tolerance level of accepting v as 0
    lambda = zeros(max_vertices);               % baricentric coordinates of the closest point

    if n == 2,
        support_mapping = @(V,x) support_mapping2d(V,x);
    else
        error("missing support mapping func");
    endif

    while true,
        aw = support_mapping(A,-v);
        bw = support_mapping(B, v);
        w = aw - bw;

        % check if v is considerable as separating axis
        if is_in(W,w) || dot(v,w) > 0,
            if DEBUG_,
                printf("v is a separating axis");
                figure(1); hold on;
                plot([a(1),b(1)],[a(2),b(2)],'g');
                figure(2); hold on;
                plot([0,v(1)],[0,v(2)],"g*");
            endif

            return; 
        endif

        [W,AW,BW,wbit] = prepare_simplex(W,AW,BW,y,w,aw,bw);

        % let W the smallest X \subseteq Y, such that v is in conv(X)
        [y,v,lambda, delta] = johnsons_distance(W,y,wbit,delta);

        if DEBUG__,
            W2 = zeros(0,n);
            first = true;
            firstInd = 0;
            for j = 1:4,
                if bitget(y,j),
                    if first, first = false; firstInd = j;
                    endif
                    W2 = [W2;W(j,:)];
                endif
            endfor
            W2 = [W2;W(firstInd,:)];
            figure(2);
            plot_polygon(W2);
            hold on;
            plot(v(1),v(2),"ro");

            a = b = zeros(1,n);
            for j = 1:max_vertex,
                a += lambda(j) * AW(j,:);
                b += lambda(j) * BW(j,:);
            endfor

            figure(1);
            hold on;
            plot(a(1),a(2),'r*');
            plot(b(1),b(2), 'b*');

            if(waitforbuttonpress != 0) DEBUG__ = false;
            endif
        endif

        % check if v is considerable as 0
        if y == 2^max_vertex-1 || dot(v,v) <= epsTol * max(diag(W * W')),
            v = zeros(1,n);

            if DEBUG__,
                printf("v is considerable as 0\n");
                figure(2);
                hold on;
                plot(v(1),v(2),"g*");
            endif

            break;
        endif

    endwhile

endfunction

function b = is_in(A,x)

    n = rows(A);
    b = false;

    for i = 1:n,
        b = approx(A(i,:),x(:)');
        if b, return;
        endif
    endfor

endfunction

function [W,AW,BW,wbit] = prepare_simplex(W,AW,BW,y,w,a,b)

    % put w into the first position of W where y's bit is 0.
    for i = 1:4,
        if !bitget(y,i),
            wbit = bitshift(1,i-1);
            W(i,:) = w(:)';
            AW(i,:) = a(:)';
            BW(i,:) = b(:)';
            return;
        endif
    endfor

endfunction

function b = approx(a,b)
    b = dot(a-b,a-b) < eps^2;
endfunction