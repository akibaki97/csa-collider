function z = aberth(p)

    DEBUG = true;

    n = length(p)-1;
    dp = polyder(p);

    if 1,
        t = 2*pi*(1:n)/n;
        z = (1.1*cos(t)+1.1*sin(t)*I);
        eps = 1e-8;
    else
        k = 4;
        m = (n - k) / 2;
        t = 2*pi*(1:m)/m + 2*pi/(2*m);
        z(1:m) = 0.7*cos(t) + 0.7*I*sin(t);
        z(m+1:2*m) = 1 ./ conj(z(1:m));
        if k > 0
            t = 2*pi*(1:k)/k;
            z(2*m + 1:n) = cos(t) + I*sin(t);
        endif
    endif

    max_it = 50;
    it = 0;

    if DEBUG,
        figure
        r1 = roots(p);
        plot(real(r1),imag(r1),'ro');
        hold on;
        axis equal;
        zplot = plot(real(z),imag(z),'bo');
        tt = 0:1/100:2*pi;
        plot(cos(tt),sin(tt),'g');
        waitforbuttonpress;
    endif

    w = inf * ones(1,n);
    b = zeros(1,n);

    while it++ < max_it,

        for k = 1:n,
            frac = polyval(p,z(k)) / polyval(dp,z(k));
            if !b(k)
                w(k) = frac / (1 - frac * srd(z,k));
                b(k) = w(k) < eps;
            end
        endfor

        if max(abs(w)) < eps, break;
        endif

        z -= w;

        if it == 15
            m = sum(b < 1);
            t = 2*pi*(1:m)/m .+ 2*pi/(2*m); 
            w(b < 1);
            z(b < 1) = 1.2*cos(t) + 1.2*I*sin(t);
        endif

        if DEBUG,
            set(zplot,'visible','off');
            zplot = plot(real(z),imag(z),'bo');
            %set(zplot,'visible','on');
            waitforbuttonpress;
        endif
    endwhile
    it
    z = transpose(z);
  
endfunction

function o = srd(z,k)

    n = length(z);
    inds = [1:k-1,k+1:n];
    o = sum(1 ./ (z(k) - z(inds)));

endfunction