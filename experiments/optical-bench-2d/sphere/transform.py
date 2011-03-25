import visual

def transform(matrix, vector):
        # convert 3D vector to 4D homogeneous vector
        vector4 = visual.array(vector.astuple() + (1,))
        # perform the actual matrix transform
        transformed4 = matrix.dot(vector4)
        # convert the 4D homogeneous vector back to 3D
        return visual.vector(transformed4[:-1] / transformed4[-1])
