import { View, } from 'react-native'
import React from 'react'
import { NativeStackNavigationProp } from '@react-navigation/native-stack'
import { RootStackParamsList } from '../../routes'
import { fontFamily } from '../../constants'
import { Text } from '../../components'


const Login = ({ navigation }: LoginScreenProps) => {
    return (
        <View>
            <Text numberOfLines={0} style={{ fontFamily: fontFamily.thin }} onPress={() => navigation.navigate('Signup')}>Login</Text>
        </View>
    )
}

export default Login

interface LoginScreenProps {
    navigation: NativeStackNavigationProp<RootStackParamsList, 'Login'>
}