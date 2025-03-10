import { View, } from 'react-native';
import React from 'react';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamsList } from '@app/routes';
import { fontFamily } from '@app/constants';
import { Text } from '@app/components';

const Login = ({ navigation }: LoginScreenProps) => {
    return (
        <View>
            <Text style={{ fontFamily: fontFamily.thin }} onPress={() => navigation.navigate('Signup')}>Login</Text>
        </View>
    );
}

export default Login;

interface LoginScreenProps {
    navigation: NativeStackNavigationProp<RootStackParamsList, 'Login'>
}
