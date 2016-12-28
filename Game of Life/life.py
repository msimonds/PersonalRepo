# Michael Simonds
# 10/3/13

# life.py
# Simulates the game of life for a given number of iterations, displaying the board at each step.

import picture
ROUNDS = 500
W = 50
H = 80

def boardSetup(board): #need to add other configurations
    boardChoice = eval(input("Choose a configuration: "))
    if boardChoice == 1:
        board[0][2]=1
        board[0][3]=1
        board[1][2]=1
        board[10][0]=1
        board[17][3]= 1
        board[28][21]=1
        board[28][22] = 1
        board[28][23]=1
        board[2][3]=1
        board[3][1]=1
        board[35][38] = 1
        board[34][39]=1
        board[35][39]=1
        board[36][39]=1
        board[34][40]=1
        
    if boardChoice == 2:
        board[0][0]=1
        board[4][4]=1
        board[0][1]=1
        board[25][40]=1
        board[25][42]=1
        board[26][41]=1
        board[26][43]=1
        board[24][41]=1
        board[1][2]=1
        board[2][2]=1
        board[3][2]= 1
        board[16][38] = 1
        board[15][39]=1
        board[16][39]=1
        board[36][39]=1
        board[15][40]=1
        board[35][38] = 1
        board[34][39]=1
        board[35][39]=1
        board[36][39]=1
        board[34][40]=1
    
    if boardChoice==3:
        for i in range(0,40,25):
            for j in range(0,70,20):
                board[i+5][j+5]=1
                board[i+5][j+6]=1
                board[i+4][j+6]=1
                board[i+6][j+6]=1
                board[i+4][j+7]=1
        board[25][40]=1
        board[25][42]=1
        board[26][41]=1
        board[26][43]=1
        board[24][41]=1
    return board

def liveCount(board,newBoard):    #counts the living cells surrounding the current cell
    for row in range(W):
        for col in range(H):
            living = 0
            for i in range(row-1, row+2):
                for j in range(col-1, col+2):
                    #print("This is i,j,H,W", i,j,H,W)
                    if i%(W)==row and j%(H)==col:
                        living = living + 0
                    elif board[i%(50)][j%(80)] == 1:
                            living = living +1
            if (living>=4) or (living<=1):
                newBoard[row][col] = 0
            elif (living == 3):
                newBoard[row][col] = 1
            else:
                newBoard[row][col] = board[row][col]
    return newBoard

def main():
# setting up board stuff
    board = []
    newBoard = []
    for i in range(W):
        board.append([0]*H)
        newBoard.append([0]*H)
    board = boardSetup(board)
    pic = picture.Picture((300,480))
    tiles = []
    pic.setOutlineColor((0,255,0))
    pic.setFillColor((0,0,0))
    for i in range(W):
        tiles.append([0]*H) 
    for x in range(W):
        for y in range(H):
            tiles[x][y] = pic.drawRectFill(x*6,y*6,6,6)
# goes through board and checks if cell is alive, then changes it to red
    for x in range(W):
        for y in range(H):
            if board[x][y] == 1:
                tiles[x][y].changeFillColor((255,0,0))
# goes through iterations of the configurations, displaying at each step
    for i in range (ROUNDS-1):
        pic.display()
        newBoard = liveCount(board,newBoard)
        for l in range(W): # sets newBoard to board then changes color of cells of board
            for p in range(H):
                board[l][p] = newBoard[l][p]
                if board[l][p] == 1:
                    tiles[l][p].changeFillColor((255,0,0))
                if board[l][p] == 0:
                    tiles[l][p].changeFillColor((0,0,0))
    pic.display()
    input()
 

main()