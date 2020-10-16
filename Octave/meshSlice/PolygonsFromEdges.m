function Pf = PolygonsFromEdges(PE)

n = rows(PE);
P = zeros(n,2*n+2);
P(:,1:4) = PE;

sizes = 2*ones(n,1);
eps = 1e-10;

for i = 1:n,
    if sizes(i) != 0,
        a1 = P(i,1:2);
        b1 = [P(i,2*sizes(i)-1),P(i,2*sizes(i))];
        if norm(a1 - b1) > eps,
            k = i;
            for j = 1:n,
                if sizes(j) != 0 && i != j,
                    a2 = P(j,1:2);
                    b2 = [P(j,2*sizes(j)-1),P(j,2*sizes(j))];
                    if norm(a1 - b2) < eps && norm(b1 - a2) < eps,
                        v = [P(k,1:2*sizes(k)),P(j,3:2*sizes(j))];
                        P(k,:) = zeros(1,2*n+2);
                        P(j,1:length(v)) = v;
                        sizes(k) = 0;
                        sizes(j) = length(v)/2;
                        break;
                    elseif norm(a1 - b2) < eps,
                        v = [P(j,1:2*sizes(j)),P(k,3:2*sizes(k))];
                        P(k,:) = zeros(1,2*n+2);
                        P(j,1:length(v)) = v;
                        sizes(k) = 0;
                        sizes(j) = length(v)/2;
                        a1 = [v(1),v(2)]; b1 = [v(2*sizes(j)-1),v(2*sizes(j))];
                        k = j;
                    elseif norm(b1 - a2) < eps,
                        v = [P(k,1:2*sizes(k)),P(j,3:2*sizes(j))];
                        P(k,:) = zeros(1,2*n+2);
                        P(j,1:length(v)) = v;
                        sizes(k) = 0;
                        sizes(j) = length(v)/2;
                        a1 = [v(1),v(2)]; b1 = [v(2*sizes(j)-1),v(2*sizes(j))];
                        k = j;
                    end
                end
            end
        end
    end
end

k = 1;
Pf = zeros(1,2*n+3);
for i = 1:n,
    if sizes(i) != 0,
        Pf(k++,:) = [sizes(i),P(i,:)];
    end
end