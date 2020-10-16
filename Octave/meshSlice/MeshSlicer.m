function PE = MeshSlicer(V,F,height,direct)

N = length(F);
PE = zeros(0,4); % polygon edges
n = 1;
x = 0; y = 0;

if direct == 1,
    x = 2; y = 3;
elseif direct == 2,
    x = 1; y = 3;
elseif direct == 3,
    x = 1; y = 2;
end

for k = 1:N,
    ind = F(k,:);
    triangle = zeros(3,3);
    triangle(1,:) = V(ind(1),:);
    triangle(2,:) = V(ind(2),:);
    triangle(3,:) = V(ind(3),:);

    pairs= [1,2,2,3,3,1];
    intersectFound = false;
    for i = 1:2:5,
        vert1 = triangle(pairs(i),:);
        vert2 = triangle(pairs(i+1),:);

        if abs(vert1(direct)-vert2(direct)) < eps,
            if abs(height - vert1(3)) < eps,
                PE(n,1:2) = [vert1(x),vert1(y)];
                PE(n,3:4) = [vert2(x),vert2(y)];
                intersectFound = true;
            end
        else
            t = (height - vert1(direct))/(vert2(direct)-vert1(direct));
            if 0 < t && t <= 1+eps,
                intersectFound = true;
                if vert1(direct) > vert2(direct),
                    PE(n,1:2) = [vert1(x),vert1(y)] + t .* ([vert2(x),vert2(y)]-[vert1(x),vert1(y)]);
                else
                    PE(n,3:4) = [vert1(x),vert1(y)] + t .* ([vert2(x),vert2(y)]-[vert1(x),vert1(y)]);
                end
            end
        end
    end
    if intersectFound, n++; end
    
end