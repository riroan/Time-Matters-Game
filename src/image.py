import cv2
import numpy as np

src = cv2.imread("2.jpg", cv2.IMREAD_GRAYSCALE)

def brightness(src):
    for i in range(200):
        img = np.copy(src)
        a = -100 + i
        img = np.clip(src + a, 0, 255).astype(np.uint8)
        cv2.imwrite("pictures2/brightness" + str(i)+".jpg",img)        
        
def saturate(src):
    for i in range(100):
        img = np.copy(src)
        a = -5 + 0.1*i
        img = np.clip((1+a)*img - 128*a,0,255).astype(np.uint8)
        cv2.imwrite("pictures/result"+str(i)+".jpg",img)
        
def filtered(src, kernel):
    img = cv2.filter2D(src, -1, kernel)
    cv2.imwrite("r.jpg",img)
    

def binomial(src):
    
    dst = cv2.Sobel(src, cv2.CV_64F, 1, 0, ksize = 3)
    img = cv2.convertScaleAbs(dst)
    for t in range(25):
        dst = np.copy(img)
        for j in range(dst.shape[0]):
            for i in range(dst.shape[1]):
                dst[j][i] = 255 - dst[j][i]
                k = (t+1) * 10
                if dst[j][i] > 255 - k:
                    dst[j][i] = 255
                elif dst[j][i] < k:
                    dst[j][i] = 0
        cv2.imwrite("pictures3/bin"+str(t)+".jpg",dst)

def edge2(src):        
    dst = cv2.Canny(src, 0, 255)
    for j in range(dst.shape[0]):
        for i in range(dst.shape[1]):
            dst[j][i] = 255 - dst[j][i]
    return dst
            
def conversion(src):
    dst = np.copy(src)
    for j in range(src.shape[0]):
        for i in range(src.shape[1]):
            dst[j][i] = 255 - src[j][i]
    return dst

def play():
    cv2.namedWindow("dst")
    
    cv2.createTrackbar("Threshold","dst",0,255,lambda x : x)
    cv2.createTrackbar("maxval","dst",0,255,lambda x : x)
    
    cv2.setTrackbarPos("Threshold", "dst", 127)
    cv2.setTrackbarPos("maxval", "dst", 255)
    
    while cv2.waitKey(1) != ord("q"):
        thresh = cv2.getTrackbarPos("Threshold","dst")
        maxval = cv2.getTrackbarPos("maxval","dst")
        _, dst = cv2.threshold(src, thresh, maxval, cv2.THRESH_BINARY)
            
        cv2.imshow("dst",dst)
        
#k = np.array([[-1,-1,-1],[-1,9,-1],[-1,-1,-1]])
#filtered(src, k)
dst = edge2(src)
cv2.imwrite("r.jpg",dst)
        
cv2.waitKey(0)
cv2.destroyAllWindows()